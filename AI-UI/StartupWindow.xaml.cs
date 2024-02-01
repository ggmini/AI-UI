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

        /// <summary>
        /// Retrieve all existing trees from output folder
        /// </summary>
        void PopulateList() {
            try {
                string[] directories = Directory.GetDirectories("output");
                foreach (string dir in directories)
                {
                    if (File.Exists($"{dir}\\tree.txt"))
                    {
                        string name = dir.Remove(0, 7); //We only want to display the tree name in the UI so we need to remove "output\" from the string
                        TreeList.Items.Add(name);
                    }
                }
            }
            catch (DirectoryNotFoundException) { } //Output Directory does not exist (yet) -> Obviously no trees exist
        }

        /// <summary>
        /// Move to Grid Prompting user for a new Tree Name
        /// </summary>
        void NewTree(object sender, RoutedEventArgs e) {
            NewGrid.Visibility = Visibility.Visible;
            NewOrLoadGrid.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Move to Grid allowing user to select an existing tree
        /// </summary>
        void LoadTree(object sender, RoutedEventArgs e) {
            LoadGrid.Visibility = Visibility.Visible;
            NewOrLoadGrid.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Load Tree selected by the user
        /// </summary>
        void LoadSelectedTree(object sender, RoutedEventArgs e) {
            ImgTree.LoadTree(File.OpenText($"output\\{TreeList.SelectedValue}\\tree.txt"));

            //Open MainWindow
            MainWindow main = new();
            main.Show();
            App.Current.MainWindow = main; //Set Main Application Window again
            ImgTree.PopulateTreeView();
            Close();
        }

        /// <summary>
        /// Makes sure that New Tree Name cannot be empty
        /// </summary>
        void NameChanged(object sender, RoutedEventArgs e) {
            if(inputBox.Text.Length > 0) ConfirmButton.IsEnabled = true; //Disable Button if prompt is empty
            else ConfirmButton.IsEnabled = false;
        }

        /// <summary>
        /// Create New Tree and Switch to Main Window
        /// </summary>
        void ConfirmNewTree(object sender, RoutedEventArgs e) {
            ImgTree.NewTree(inputBox.Text);
            MainWindow main = new();
            main.Show();
            App.Current.MainWindow = main;
            Close();
        }

        /// <summary>
        /// Returns user to first menu
        /// </summary>
        void BackToNewOrLoad(object sender, RoutedEventArgs e) {
            NewGrid.Visibility = Visibility.Hidden;
            LoadGrid.Visibility = Visibility.Hidden;
            NewOrLoadGrid.Visibility = Visibility.Visible;
        }
        
        /// <summary>
        /// Makes sure user cannot attempt to load a tree before he has selected one
        /// </summary>
        void SelectionChanged(object sender, RoutedEventArgs e) {
            if(TreeList.SelectedItem == null) LoadButton.IsEnabled = false;
            else LoadButton.IsEnabled = true;
        }
    }
}
