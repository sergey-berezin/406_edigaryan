using Ookii.Dialogs.Wpf;
using System.Collections.Generic;
using System.Data;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.Linq;

namespace WPF1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainViewModel = new ViewModel();
            DataContext = MainViewModel;
            MainViewModel.PropertyChanged += MainViewModel_PropertyChanged;
        }

        private void MainViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainViewModel.ImagesPaths))
            {
                DrawGrid();
            }

            if (e.PropertyName == nameof(MainViewModel.Completed) && MainViewModel.Completed)
            {
                DisplayData();
            }
        }

        private List<BitmapImage> images = new List<BitmapImage>();

        public ViewModel MainViewModel { get; set; }

        private void OnOpenButtonClicked(object sender, RoutedEventArgs e)
        {
            var dlg = new VistaFolderBrowserDialog();

            if (dlg.ShowDialog() == true)
            {
                MainViewModel.Path = dlg.SelectedPath;
            }
        }

        private async void StartButtonClick(object sender, RoutedEventArgs e)
        {
            if (MainViewModel.Path == null)
            {
                MessageBox.Show("Выберите папку.", "Папка не выбрана.");
                return;
            }

            if (MainViewModel.ImagesPaths == null || MainViewModel.ImagesPaths.Length == 0)
            {
                MessageBox.Show("В папке нет картинок.", "В папке нет картинок.");
                return;
            }

            await MainViewModel.StartButtonClicked();
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            MainViewModel.Cancel();
        }

        private void ClearButtonClick(object sender, RoutedEventArgs e)
        {
            MainViewModel.Clear();
            table.RowDefinitions.Clear();
            table.ColumnDefinitions.Clear();
            table.Children.Clear();
            images.Clear();
            ClearLabels();
        }

        public void ClearLabels()
        {
            var itemsToRemove = new List<object>();
            foreach (var item in table.Children)
            {
                if (item is Label)
                {
                    itemsToRemove.Add(item);
                }
            }

            foreach (var itemToRemove in itemsToRemove)
            {
                table.Children.Remove((UIElement)itemToRemove);
            }
        }

        private void AddUnitGrid()
        {
            table.RowDefinitions.Add(new RowDefinition()
            {
                Height = new GridLength(1, GridUnitType.Star)
            });

            table.ColumnDefinitions.Add(new ColumnDefinition()
            {
                Width = new GridLength(1, GridUnitType.Star)
            });
        }

        private void DrawGrid()
        {
            AddUnitGrid();
            foreach (var (path, i) in MainViewModel.ImagesPaths.Select((x, i) => (x, i)))
            {
                AddUnitGrid();

                var uri = new Uri(path);
                var bitmap = new BitmapImage(uri);

                PutImageOnGrid(bitmap, 0, i + 1);
                PutImageOnGrid(bitmap, i + 1, 0);
                images.Add(bitmap);
            }
        }

        public void PutImageOnGrid(BitmapImage bitmap, int col, int row)
        {
            var image = new Image();
            image.Source = bitmap;
            Grid.SetColumn(image, col);
            Grid.SetRow(image, row);
            table.Children.Add(image);
        }

        private void PutLabelOnGrid(Label label, int column, int row)
        {
            table.Children.Add(label);
            Grid.SetColumn(label, column);
            Grid.SetRow(label, row);
        }

        private void DisplayData()
        {
            for (int i = 0; i < images.Count; ++i)
            {
                for (int j = 0; j < images.Count; ++j)
                {
                    var label = new Label()
                    {
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        FontSize = 12
                    };

                    var distance = MainViewModel.Distances[i, j];
                    var similarities = MainViewModel.Similarities[i, j];

                    label.Content = $"Distance: {distance:0.00}\nSimilarity: {similarities:0.00}";

                    PutLabelOnGrid(label, i + 1, j + 1);
                }
            }
        }
    }
}
