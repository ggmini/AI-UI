using System.Windows;
using System;
using Data_Structure;
using System.Windows.Controls;
using System.Windows.Input;
using static AI_UI.ResponseStruct;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows.Media.Imaging;

namespace AI_UI {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        
        //Reference to Loggin Window
        LogWindow log;
        public LogWindow Log { get => log; }

        public MainWindow() {
            InitializeComponent();

            //Create and Open Log Window
            log = new();
            log.Show();
            Topmost = true;
            Topmost = false;

            Controller.SetLog(log);

            //Initialize Stable Diffusion Interface
            StableInterface.Initialize(this);

            ChangeStatusMessage("Ready...");
        }

        void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            ImgTree.SaveTree();
            log.SaveLog();
            log.Close();
        }

        /// <summary>
        /// Change the Message visible in the Status Bar
        /// </summary>
        /// <param name="value">New Message to be displaye</param>
        public void ChangeStatusMessage(string value) {
            StatusText.Text = value;
        }

        /// <summary>
        /// Change Progressbar Progress
        /// </summary>
        /// <param name="value">New Value of the Progressbar Progress</param>
        public void ChangeProgressbar(double value) {
            ProgressBar.Value = value;
        }

        private void Path_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            GenerateButton.IsEnabled = false;

            //If Resolution Boxes are empty, use default      
            if (!Int32.TryParse(WidthBox.Text, out int width)) width = 512;
            if (!Int32.TryParse(HeightBox.Text, out int height)) height = 512;
            string prompt;
            string negativePrompt;
            if (PromptBox.Text == "Prompt") prompt = "";
            else prompt = PromptBox.Text;
            if (NegativePromptBox.Text == "Negative Prompt") negativePrompt = "";
            else negativePrompt = NegativePromptBox.Text;

            StableInterface.GenerateTxt2Img(prompt, negativePrompt, (long)SeedBox.Value, (int)StepsSlider.Value, (int)BatchSizeSlider.Value, width, height);
        }

        private void PathRandom_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //GenerateButton.IsEnabled = false; <---- durch Bild ersetzen

            SeedBox.Value = -1;
        }

        private void Path_GoBack(object sender, MouseButtonEventArgs e)
        {
            StartupWindow startupWindow = new StartupWindow();
            startupWindow.Show();
            App.Current.MainWindow = startupWindow;
            Close();
        }


        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            if (textBox.Text == textBox.Tag.ToString())
            {
                textBox.Text = string.Empty;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = textBox.Tag.ToString();
            }
        }



        #region ImageViewer
        List<BitmapImage> imgs;
        List<ResponseStruct.imgInfo> imgInfos;
        int currentImage = 0;

        public void OpenNode(ImgNode node) {
            ImageGrid.Visibility = Visibility.Visible;
            TreeGrid.Visibility = Visibility.Hidden;
            LoadNode(node);
        }

        public void CloseNode() {
            ImageGrid.Visibility = Visibility.Hidden;
            TreeGrid.Visibility = Visibility.Visible;
        }

        void LoadNode(ImgNode node) {
            imgs = new List<BitmapImage>();
            imgInfos = new List<ResponseStruct.imgInfo>();

            foreach (var i in node.imgId)
            {
                imgs.Add(new(new(Path.GetFullPath($"output\\{ImgTree.CurrentTreeName}\\{i}.png"))));
                string imageString = new StreamReader($"output\\{ImgTree.CurrentTreeName}\\{i}.txt").ReadToEnd();
                imgInfos.Add(JsonSerializer.Deserialize<ResponseStruct.imgInfo>(imageString));
            }
            currentImage = 0;
            LoadImage();
        }

        void LoadImage() {
            ImageView.Source = imgs[currentImage];
            ResponseStruct.imgInfo imgInfo = imgInfos[currentImage];
            ImageInfo.Text = $"Prompt: {imgInfo.prompt}\nNegative Prompt: {imgInfo.negative_prompt}\nSeed: {imgInfo.seed}\nSteps: {imgInfo.steps}\n" +
                $"Resolution: {imgInfo.width}x{imgInfo.height}\nDenoising Strength: {imgInfo.denoising_strength}";
        }

        void NextClick(object sender, RoutedEventArgs e) {
            if (currentImage == imgs.Count - 1) //Are we on the last image
                currentImage = 0; //Skip back to front
            else currentImage++;
            LoadImage();
        }

        void BackClick(object sender, RoutedEventArgs e) {
            if (currentImage == 0) //Are we on the firsrt image
                currentImage = imgs.Count - 1; //Skip back to back
            else currentImage--;
            LoadImage();
        }
        #endregion

        void BackToTreeButton_Click(object sender, RoutedEventArgs e) {
            CloseNode();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
