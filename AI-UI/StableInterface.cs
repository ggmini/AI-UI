using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading;
using System.Windows.Threading;
using System.Windows;

namespace  AI_UI{
    public static class StableInterface {

        public enum State { Inactive, Active }

        static State status = State.Inactive;
        public static State Status { get => status; }

        public struct requestStruct {
            public string prompt { get; set; }
            public string negative_prompt { get; set; }
            public int seed { get; set; }
            public int steps { get; set; }
            public int width { get; set; }
            public int height { get; set; }
            public int batch_size { get; set; }
        }

        public struct responseStruct {
            public string[] images { get; set; }
        }

        public struct progressStruct {
            public double progress { get; set; }
        }

        //Our Http Client to access the Automatic1111 API
        static HttpClient client;

        // References to other Windows
        static MainWindow _main;
        static LogWindow _log;

        /// <summary>
        /// Initializes the Stable Diffusion Interface
        /// </summary>
        /// <param name="main">Current Main Window</param>
        /// <param name="log">Current Log Window</param>
        public static void Initialize(MainWindow main, LogWindow log) {
            //Create new HTTP Client
            client = new HttpClient();
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            _main = main;
            _log = log;

            log.WriteToLog("Stable Diffusion Interface Started");
        }

        /// <summary>
        /// Generate a new Image using txt2img
        /// </summary>
        /// <param name="prompt">Prompt</param>
        /// <param name="negativePrompt">Negative Prompt</param>
        /// <param name="seed">Seed to be used (-1 for random)</param>
        /// <param name="steps">How many Steps should be generated</param>
        /// <param name="batch_size">How many images should be generated</param>
        /// <param name="width">Image width in pixels</param>
        /// <param name="height">Image height in pixels</param>
        public static void Generate(string prompt, string negativePrompt, int seed, int steps, int batch_size, int width, int height) {
            requestStruct requestStruct = new() {
                prompt = prompt,
                negative_prompt = negativePrompt,
                seed = seed,
                steps = steps,
                batch_size = batch_size,
                width = width,
                height = height
            };

            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += GenerateWork;
            worker.RunWorkerAsync(requestStruct);
        }

        /// <summary>
        /// Background Worker to Generate Image
        /// </summary>
        static async void GenerateWork(object sender, DoWorkEventArgs e) {
            status = State.Active;

            requestStruct requestStruct = (requestStruct)e.Argument;

            var jsonOptions = new JsonSerializerOptions() {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            };
            JsonContent requestBody = JsonContent.Create(requestStruct, requestStruct.GetType(), options: jsonOptions);

            Thread t = new Thread(() => ProgressObserver());
            t.Start();

            Application.Current.Dispatcher.Invoke(() => _log.WriteToLog("Sending Txt2Img Request"));
            await Application.Current.Dispatcher.InvokeAsync(async () => _log.WriteToLog(await requestBody.ReadAsStringAsync())); ;
            var result = await client.PostAsync("http://127.0.0.1:7860/sdapi/v1/txt2img", requestBody);
            Application.Current.Dispatcher.Invoke(() => _log.WriteToLog("Request Finished with " + (int)result.StatusCode + " " + result.StatusCode));
            Application.Current.Dispatcher.Invoke(() => _main.ChangeStatusMessage("Generation Completed"));
            Console.WriteLine(result.ReasonPhrase);
            string responseBody = await result.Content.ReadAsStringAsync();

            responseStruct response = JsonSerializer.Deserialize<responseStruct>(responseBody);

            if (response.images != null)
                SaveImage(response.images);
            else MessageBox.Show("Error", "No Images Found in Response");

            status = State.Inactive;
            Application.Current.Dispatcher.Invoke(() => _main.ChangeStatusMessage("Ready"));
            Application.Current.Dispatcher.Invoke(() => _main.GenerateButton.IsEnabled = true);
        }

        /// <summary>
        /// Save the Generated Images
        /// </summary>
        /// <param name="base64string">Collection of images as Base64 String</param>
        static void SaveImage(string[] base64string) {
            string filePath = $"output\\" + DateTime.Now.ToString("dd.MM.yyyy hh-mm-ss") + "\\";
            Application.Current.Dispatcher.Invoke(() => _log.WriteToLog("Saving images to " + filePath));
            Directory.CreateDirectory(filePath);
            int i = 0;
            foreach (var image in base64string)
                File.WriteAllBytes(filePath + i++ + ".png", Convert.FromBase64String(image));
            Process.Start("explorer.exe", filePath);
        }

        /// <summary>
        /// Observe Image Generation Progress
        /// </summary>
        static void ProgressObserver() {
            HttpClient observerClient = new(); //Using second client to prevent cross threading issues
            while (status == State.Active) {
                CheckProgress(observerClient);
                Thread.Sleep(500);
            }
        }

        /// <summary>
        /// Send Request to API to Check for Image Progress
        /// </summary>
        /// <param name="client">HttpClient to be used for the Request</param>
        static async void CheckProgress(HttpClient client) {
            try {
                Application.Current.Dispatcher.Invoke(() => _log.WriteToLog("Requesting Progress"));
                var response = await client.GetAsync("http://127.0.0.1:7860/sdapi/v1/progress");
                string responseString = await response.Content.ReadAsStringAsync();
                //Dispatcher.Invoke(() => log.WriteToLog("Progress Response Received: " + responseString));
                progressStruct progressStruct = JsonSerializer.Deserialize<progressStruct>(responseString);
                double progressInPercent = progressStruct.progress * 100;
                Application.Current.Dispatcher.Invoke(() => _log.WriteToLog("Progress Response Received: " + progressInPercent.ToString()));
                Application.Current.Dispatcher.Invoke(() => _main.ChangeStatusMessage($"Progress: {progressInPercent.ToString("0.##")}%"));
                Application.Current.Dispatcher.Invoke(() => _main.ChangeProgressbar(progressInPercent));
            } catch (Exception e) { Application.Current.Dispatcher.Invoke(() => _log.WriteToLog("Progress Request Failed: " + e.Message)); }
        }

    }
}
