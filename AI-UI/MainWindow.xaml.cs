using System.Windows;
using System;
using Data_Structure;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;

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
        /// Called when Steps Slider is moved
        /// </summary>
        //void StepsSlider_Changed(object sender, RoutedPropertyChangedEventArgs<double> e) {
        //    StepsText.Text = "Steps: " + e.NewValue.ToString();
        //}

        /// <summary>
        /// Called when Batch Size Slider is moved
        /// </summary>
        //void BatchSizeSlider_Changed(object sender, RoutedPropertyChangedEventArgs<double> e) {
        //    BatchSizeText.Text = "Batch Size: " + e.NewValue.ToString();
        //}

        /// <summary>
        /// Called when Generate Button is clicked
        /// </summary>
        void Generate_Click(object sender, RoutedEventArgs e) {
            GenerateButton.IsEnabled = false;

            StableInterface.GenerateTxt2Img(PromptBox.Text, NegativePromptBox.Text, long.Parse(SeedBox.Text), (int)StepsSlider.Value, (int)BatchSizeSlider.Value,
                Convert.ToInt32(WidthBox.Text), Convert.ToInt32(HeightBox.Text));
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

            StableInterface.GenerateTxt2Img(PromptBox.Text, NegativePromptBox.Text, (int)SeedBox.Value, (int)StepsSlider.Value, (int)BatchSizeSlider.Value,
                Convert.ToInt32(WidthBox.Text), Convert.ToInt32(HeightBox.Text));

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
    }
}
