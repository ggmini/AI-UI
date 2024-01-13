using System.IO;
using System;
using System.Windows;

namespace AI_UI
{
    /// <summary>
    /// Interaction logic for LogWindow.xaml
    /// </summary>
    public partial class LogWindow : Window
    {
        public LogWindow() {
            InitializeComponent();

            LogBlock.Text = $"[{DateTime.Now.ToString("hh-mm-ss")}] Log Initialized";
        }

        /// <summary>
        /// Write a string to the Log
        /// </summary>
        /// <param name="output">String to write to log</param>
        public void WriteToLog(string output) {
            LogBlock.Text = $"[{DateTime.Now.ToString("hh-mm-ss")}] {output}\n{LogBlock.Text}";
        }

        /// <summary>
        /// Save the Log to a txt file
        /// </summary>
        public void SaveLog() {
            string filePath = $"log\\{DateTime.Now.ToString("dd.MM.yyyy hh-mm-ss")}.txt";
            Directory.CreateDirectory("log");
            using (StreamWriter writer = new StreamWriter(filePath)) {
                writer.Write(LogBlock.Text);
                writer.Close();
            }
            
        }

    }
}
