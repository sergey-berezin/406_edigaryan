using System.Windows;

namespace WPF2
{
    /// <summary>
    /// Interaction logic for DataBaseViewer.xaml
    /// </summary>
    public partial class DataBaseViewer : Window
    {
        public DataBaseViewer(MainViewModel mainViewModel)
        {
            InitializeComponent();
            DataContext = mainViewModel;
        }

        private void DeleteSelectedImage(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as MainViewModel;
            vm?.DeleteSelectedImage();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as MainViewModel;
            vm?.ClearDb();
        }
    }
}
