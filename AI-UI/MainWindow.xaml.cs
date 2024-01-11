﻿using System.Windows;
using System;

namespace AI_UI {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        
        //Reference to Loggin Window
        LogWindow log;

        public MainWindow() {
            InitializeComponent();

            //Create and Open Log Window
            log = new();
            log.Show();

            //Initialize Stable Diffusion Interface
            StableInterface.Initialize(this, log);

            ChangeStatusMessage("Ready...");
        }

        /// <summary>
        /// Called when Steps Slider is moved
        /// </summary>
        void StepsSlider_Changed(object sender, RoutedPropertyChangedEventArgs<double> e) {
            StepsText.Text = "Steps: " + e.NewValue.ToString();
        }

        /// <summary>
        /// Called when Batch Size Slider is moved
        /// </summary>
        void BatchSizeSlider_Changed(object sender, RoutedPropertyChangedEventArgs<double> e) {
            BatchSizeText.Text = "Batch Size: " + e.NewValue.ToString();
        }

        /// <summary>
        /// Called when Generate Button is clicked
        /// </summary>
        void Generate_Click(object sender, RoutedEventArgs e) {
            GenerateButton.IsEnabled = false;

            StableInterface.Generate(PromptBox.Text, NegativePromptBox.Text, Convert.ToInt32(SeedBox.Text), (int)StepsSlider.Value, (int)BatchSizeSlider.Value,
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
        
    }
}
