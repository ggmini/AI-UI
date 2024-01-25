using Data_Structure;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace AI_UI {
    /// <summary>
    /// Interaction logic for Startup_Window.xaml
    /// </summary>
    public partial class StartupWindow : Window {

        //Grids used by this Window
        Grid NewOrLoadGrid;
        Grid NewGrid;

        //Need Access to these from Code Behind
        TextBox inputBox;
        Button confirmButton;

        public StartupWindow() {
            InitializeComponent();
            GenerateGrids();

            MainGrid.Children.Add(NewOrLoadGrid);
        }

        /// <summary>
        /// Move to Tree Name Prompt
        /// </summary>
        void NewTree(object sender, RoutedEventArgs e) {
            ChangeLayout(NewGrid);
        }

        /// <summary>
        /// Let User Select Tree to Load
        /// </summary>
        void LoadTree(object sender, RoutedEventArgs e) {
            //Let User Select File
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.ShowDialog();
            try {
                ImgTree.LoadTree(File.OpenText(ofd.FileName)); //Open File and Pass Stream Reader to ImgTree
            } catch (ArgumentException) { return; } //if user closes file dialog return to menu

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
            if(inputBox.Text.Length > 0) confirmButton.IsEnabled = true;
            else confirmButton.IsEnabled = false;
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
            ChangeLayout(NewOrLoadGrid);
        }

        /// <summary>
        /// Generate the Layouts we will be using in this window
        /// </summary>
        void GenerateGrids() {
            #region NewOrLoadGrid
            NewOrLoadGrid = new();
            ColumnDefinition column = new();
            NewOrLoadGrid.ColumnDefinitions.Add(column);
            column = new();
            NewOrLoadGrid.ColumnDefinitions.Add(column);

            Button NewButton = new() {
                Content = "New Tree",
                FontSize = 36,
                Margin = new Thickness(10, 10, 5, 10)
            };
            NewButton.Click += NewTree;
            Button LoadButton = new() {
                Content = "Load Tree",
                FontSize = 36,
                Margin = new Thickness(5, 10, 10, 10)
            };
            LoadButton.Click += LoadTree;

            NewOrLoadGrid.Children.Add(NewButton);
            NewOrLoadGrid.Children.Add(LoadButton);
            Grid.SetColumn(NewButton, 0);
            Grid.SetColumn(LoadButton, 1);
            #endregion

            #region NewGrid
            NewGrid = new() { Margin = new Thickness(10, 10, 10, 10) };
            RowDefinition row = new();
            NewGrid.RowDefinitions.Add(row);
            row = new();
            NewGrid.RowDefinitions.Add(row);

            inputBox = new();
            inputBox.TextChanged += NameChanged;
            NewGrid.Children.Add(inputBox);
            Grid.SetRow(inputBox, 0);

            confirmButton = new() {
                Content = "Confirm",
                FontSize = 20,
                Width = 100,
                Height = 50,
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            confirmButton.Click += ConfirmNewTree;
            Button BackButton = new() {
                Content = "Back",
                FontSize = 20,
                Width = 100,
                Height = 50,
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            BackButton.Click += BackToNewOrLoad;

            NewGrid.Children.Add(confirmButton);
            NewGrid.Children.Add(BackButton);
            Grid.SetRow(confirmButton, 1);
            Grid.SetRow(BackButton, 1);
            #endregion
        }

        /// <summary>
        /// Change to a different Layout
        /// </summary>
        /// <param name="grid">Grid to change to</param>
        void ChangeLayout(Grid grid) {
            MainGrid.Children.Clear();
            MainGrid.Children.Add(grid);
        }
    }
}
