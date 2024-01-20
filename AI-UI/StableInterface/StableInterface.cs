using System.ComponentModel;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading;
using System.Windows;
using Data_Structure;

namespace  AI_UI{
    public static class StableInterface {

        public enum State { Inactive, Active }

        static State status = State.Inactive;
        public static State Status { get => status; }

        //Our Http Client to access the Automatic1111 API
        static HttpClient client;

        // References to other Windows
        static MainWindow _main;

        //url for the sdapi
        static string url = "http://127.0.0.1:7860/sdapi/v1/";

        /// <summary>
        /// Initializes the Stable Diffusion Interface
        /// </summary>
        /// <param name="main">Current Main Window</param>
        /// <param name="log">Current Log Window</param>
        public static void Initialize(MainWindow main) {
            //Create new HTTP Client
            client = new HttpClient();
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            _main = main;

            Controller.WriteToLog("Stable Diffusion Interface Started");
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
        public static void GenerateTxt2Img(string prompt, string negativePrompt, int seed, int steps, int batch_size, int width, int height) {
            RequestStruct requestStruct = new() {
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
        /// Generate new Image using img2img
        /// </summary>
        /// <param name="prompt">Prompt</param>
        /// <param name="negativePrompt">Negative Prompt</param>
        /// <param name="seed">Seed to be used (-1 for random)</param>
        /// <param name="steps">How many Steps should be generated</param>
        /// <param name="batch_size">How many images should be generated</param>
        /// <param name="width">Image width in pixels</param>
        /// <param name="height">Image height in pixels</param>
        /// <param name="images">Collection of input images</param>
        public static void GenerateImg2Img(string prompt, string negativePrompt, int seed, int steps, int batch_size, int width, int height, string[] images) {
            RequestStruct requestStruct = new() {
                prompt = prompt,
                negative_prompt = negativePrompt,
                seed = seed,
                steps = steps,
                batch_size = batch_size,
                width = width,
                height = height,
                init_images = images,
                denoising_strength = 0.75 //Copied from the API's Example Request
            };

            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += GenerateWork;
            worker.RunWorkerAsync(requestStruct);
        }

        /// <summary>
        /// Background Worker to Generate Image
        /// </summary>
        static async void GenerateWork(object sender, DoWorkEventArgs e) {
            //Set State active
            status = State.Active;

            RequestStruct requestStruct = (RequestStruct)e.Argument;

            //Serialize the Request
            var jsonOptions = new JsonSerializerOptions() {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            };
            JsonContent requestBody = JsonContent.Create(requestStruct, requestStruct.GetType(), options: jsonOptions);

            //Start Progress Observer
            Thread t = new Thread(() => ProgressObserver());
            t.Start();

            string command;
            //Decide if txt2img or img2img request is to be sent
            if (requestStruct.init_images == null)
                command = "txt2img";
            else command = "img2img";

            //Send Post Request to Generate Image
            Application.Current.Dispatcher.Invoke(() => Controller.WriteToLog($"Sending {command} Request"));
            await Application.Current.Dispatcher.InvokeAsync(async () => Controller.WriteToLog(await requestBody.ReadAsStringAsync())); 

            try {
                var result = await client.PostAsync(url + command, requestBody);

                //Show Server Response
                Application.Current.Dispatcher.Invoke(() => Controller.WriteToLog("Request Finished with " + (int)result.StatusCode + " " + result.StatusCode));
                Application.Current.Dispatcher.Invoke(() => _main.ChangeStatusMessage("Generation Completed"));
                if (result.ReasonPhrase != null) Application.Current.Dispatcher.Invoke(() => Controller.WriteToLog(result.ReasonPhrase));

                //Deserialize Response
                string responseBody = await result.Content.ReadAsStringAsync();
                ResponseStruct response = JsonSerializer.Deserialize<ResponseStruct>(responseBody);
                ResponseStruct.batchImgInfo batchInfo = response.ExtractImgInfo();

                //Save Images
                Application.Current.Dispatcher.Invoke(() => Controller.WriteToLog("Saving images"));
                if (response.images != null)
                    ImgTree.SaveBatch(response.images, batchInfo, response.imgInfos);
                else MessageBox.Show("Error", "No Images Found in Response!\nCheck the log.");
            } catch (HttpRequestException) {
                MessageBox.Show("Could not connect to Stable Diffusion. Is the WebUI running?", "Error", MessageBoxButton.OK, MessageBoxImage.Error); //Show Error Message if couldnt connect to API
            }

            //Set State to inactive
            status = State.Inactive;
            Application.Current.Dispatcher.Invoke(() => _main.ChangeStatusMessage("Ready"));
            Application.Current.Dispatcher.Invoke(() => _main.GenerateButton.IsEnabled = true);
        }

        /// <summary>
        /// Observe Image Generation Progress
        /// </summary>
        static void ProgressObserver() {
            HttpClient observerClient = new(); //Using second client to prevent cross threading issues
            while (status == State.Active) {
                CheckProgress(observerClient);
                Thread.Sleep(500);
            } Application.Current.Dispatcher.Invoke(() => _main.ChangeProgressbar(0));
        }

        /// <summary>
        /// Send Request to API to Check for Image Progress
        /// </summary>
        /// <param name="client">HttpClient to be used for the Request</param>
        static async void CheckProgress(HttpClient client) {
            try {
                Application.Current.Dispatcher.Invoke(() => Controller.WriteToLog("Requesting Progress"));
                var response = await client.GetAsync("http://127.0.0.1:7860/sdapi/v1/progress");
                string responseString = await response.Content.ReadAsStringAsync();
                //Dispatcher.Invoke(() => log.WriteToLog("Progress Response Received: " + responseString));
                ProgressStruct progressStruct = JsonSerializer.Deserialize<ProgressStruct>(responseString);
                double progressInPercent = progressStruct.progress * 100;
                Application.Current.Dispatcher.Invoke(() => Controller.WriteToLog("Progress Response Received: " + progressInPercent.ToString()));
                Application.Current.Dispatcher.Invoke(() => _main.ChangeStatusMessage($"Progress: {progressInPercent.ToString("0.##")}%"));
                Application.Current.Dispatcher.Invoke(() => _main.ChangeProgressbar(progressInPercent));
            } catch (Exception e) { Application.Current.Dispatcher.Invoke(() => Controller.WriteToLog("Progress Request Failed: " + e.Message)); }
        }

    }
}
