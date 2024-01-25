using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Media.Imaging;
using Data_Structure;

namespace AI_UI
{
    /// <summary>
    /// Interaction logic for ImageViwer.xaml
    /// </summary>
    public partial class ImageViewer : Window {

        List<BitmapImage> imgs;
        List<ResponseStruct.imgInfo> imgInfos; 
        int currentImage;

        public ImageViewer(ImgNode node) {
            InitializeComponent();
            LoadNode(node);
        }

        void OnClose(object sender, System.ComponentModel.CancelEventArgs e) {
            ImgTree.ImageViewerOpen = false;
        }

        public void LoadNode(ImgNode node) {
            imgs = new List<BitmapImage>();
            imgInfos = new List<ResponseStruct.imgInfo>();

            foreach(var i in node.imgId) {
                imgs.Add(new(new(Path.GetFullPath($"output\\{ImgTree.CurrentTreeName}\\{i}.png"))));
                string imageString = new StreamReader($"output\\{ImgTree.CurrentTreeName}\\{i}.txt").ReadToEnd();
                Controller.WriteToLog(imageString);
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
    }
}
