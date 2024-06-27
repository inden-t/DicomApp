using System.Windows;
using System.Windows.Controls;
using DicomApp.ViewModels;
using DicomApp.Views;

namespace DicomApp
{
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel _viewModel;

        public MainWindow(MainWindowViewModel viewModel,
            ImageViewer imageViewer)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;

            // ImageContainer の中に ImageViewer を配置する
            var imageContainer = (ContentControl)FindName("ImageContainer");
            imageContainer.Content = imageViewer;
        }
    }
}
