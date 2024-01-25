﻿using System.Windows;
using System;
using Data_Structure;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
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
            Topmost = true;
            Topmost = false;

            Controller.SetLog(log);

            //Initialize Stable Diffusion Interface
            StableInterface.Initialize(this);

            ChangeStatusMessage("Ready...");
        }

        void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            ImgTree.SaveTree();
            ImgTree.ImageViewer.Close();
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
            int width;
            int height;            
            if (!Int32.TryParse(WidthBox.Text, out width)) width = 512;
            if (!Int32.TryParse(HeightBox.Text, out height)) height = 512;

            StableInterface.GenerateTxt2Img(PromptBox.Text, NegativePromptBox.Text, (long)SeedBox.Value, (int)StepsSlider.Value, (int)BatchSizeSlider.Value,
                width, height);

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
