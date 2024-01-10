using System.Windows;
using System.Net.Http;
using System.Net.Http.Json;
using System;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Diagnostics;
using System.IO;
using System.Windows.Threading;
using System.ComponentModel;

namespace AI_UI {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        struct requestStruct {
            public string prompt { get; set; }
            public string negative_prompt { get; set; }
            public int seed { get; set; }
            public int steps { get; set; }
            public int width { get; set; }
            public int height { get; set; }
            public int batch_size { get; set; }
        }

        struct responseStruct {
            public string[] images { get; set; }
        }

        struct progressStruct {
            public double progress;
        }

        public MainWindow() {
            InitializeComponent();

            ChangeStatusMessage("Ready...");
        }

        void StepsSlider_Changed(object sender, RoutedPropertyChangedEventArgs<double> e) {
            StepsText.Text = "Steps: " + e.NewValue.ToString();
        }

        void BatchSizeSlider_Changed(object sender, RoutedPropertyChangedEventArgs<double> e) {
            BatchSizeText.Text = "Batch Size: " + e.NewValue.ToString();
        }

        void Generate_Click(object sender, RoutedEventArgs e) {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += Generate;
            worker.RunWorkerAsync(worker);
            
        }

        async void Generate(object sender, DoWorkEventArgs e) {
            requestStruct request = new();

            Dispatcher.Invoke(() => {
                request.prompt = PromptBox.Text;
                request.negative_prompt = NegativePromptBox.Text;
                request.seed = Convert.ToInt32(SeedBox.Text);
                request.steps = (int)StepsSlider.Value;
                request.batch_size = (int)BatchSizeSlider.Value;
                request.width = Convert.ToInt32(WidthBox.Text);
                request.height = Convert.ToInt32(HeightBox.Text);
            });

            var jsonOptions = new JsonSerializerOptions() {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            };
            JsonContent requestBody = JsonContent.Create(request, request.GetType(), options: jsonOptions);

            HttpClient client = new();
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            MessageBox.Show(await requestBody.ReadAsStringAsync(), "Request Body");
            ChangeStatusMessage("Sending Request");
            var result = await client.PostAsync("http://127.0.0.1:7860/sdapi/v1/txt2img", requestBody);
            ChangeStatusMessage("Request Finished with " + (int)result.StatusCode + " " + result.StatusCode);
            Console.WriteLine(result.ReasonPhrase);
            string responseBody = await result.Content.ReadAsStringAsync();

            responseStruct response = JsonSerializer.Deserialize<responseStruct>(responseBody);

            if (response.images != null)
                SaveImage(response.images);
            else MessageBox.Show("Error", "No Images Found in Response");
        }

        static void SaveImage(string[] base64string) {
            string filePath = $"output\\" + DateTime.Now.ToString("dd.MM.yyyy hh-mm-ss") + "\\";
            Directory.CreateDirectory(filePath);
            int i = 0;
            foreach (var image in base64string)
                File.WriteAllBytes(filePath + i++ + ".png", Convert.FromBase64String(image));
            Process.Start("explorer.exe", filePath);
        }

        public void ChangeStatusMessage(string output) {
            Dispatcher.Invoke(() =>
            StatusText.Text = output
            );
        }
    }
}
