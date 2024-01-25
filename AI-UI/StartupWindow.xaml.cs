using Data_Structure;
using System.IO;
using System.Windows;

namespace AI_UI {
    /// <summary>
    /// Interaction logic for Startup_Window.xaml
    /// </summary>
    public partial class StartupWindow : Window {

        public StartupWindow() {
            InitializeComponent();

            PopulateList();
        }

        void PopulateList() {
            string[] directories = Directory.GetDirectories("output");
            foreach (string dir in directories) {
                if(File.Exists($"{dir}\\tree.txt"))
                    TreeList.Items.Add(dir);
            }
        }

        /// <summary>
        /// Move to Tree Name Prompt
        /// </summary>
        void NewTree(object sender, RoutedEventArgs e) {
            NewGrid.Visibility = Visibility.Visible;
            NewOrLoadGrid.Visibility = Visibility.Hidden;
        }

        void LoadTree(object sender, RoutedEventArgs e) {
            LoadGrid.Visibility = Visibility.Visible;
            NewOrLoadGrid.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Let User Select Tree to Load
        /// </summary>
        void LoadSelectedTree(object sender, RoutedEventArgs e) {
            ImgTree.LoadTree(File.OpenText(TreeList.SelectedValue.ToString() + "\\tree.txt"));

            //Open MainWindow
            MainWindow main = new();
            main.Show();
            App.Current.MainWindow = main;
            ImgTree.PopulateTreeView();
            Close();
        }

        /// <summary>
        /// Makes sure that New Tree Name cannot be empty
        /// </summary>
        void NameChanged(object sender, RoutedEventArgs e) {
            if(inputBox.Text.Length > 0) ConfirmButton.IsEnabled = true;
            else ConfirmButton.IsEnabled = false;
        }

        /// <summary>
        /// Create New Tree and Switch to Main Window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ConfirmNewTree(object sender, RoutedEventArgs e) {
            ImgTree.NewTree(inputBox.Text);
            MainWindow main = new();
            main.Show();
            App.Current.MainWindow = main;
            Close();
        }

        void BackToNewOrLoad(object sender, RoutedEventArgs e) {
            NewGrid.Visibility = Visibility.Hidden;
            LoadGrid.Visibility = Visibility.Hidden;
            NewOrLoadGrid.Visibility = Visibility.Visible;
        }

        void SelectionChanged(object sender, RoutedEventArgs e) {
            if(TreeList.SelectedItem == null) LoadButton.IsEnabled = false;
            else LoadButton.IsEnabled = true;
        }
    }
}
