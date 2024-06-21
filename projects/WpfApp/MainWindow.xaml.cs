using System.Windows;
using System.Windows.Controls;
using DICOMViewer.ViewModels;
using DICOMViewer.Views;

namespace DICOMViewer
{
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel _viewModel;

        public MainWindow(MainWindowViewModel viewModel,
            ImageViewer imageViewer)
        {
            InitializeComponent();
            _viewModel = viewModel;

            _viewModel.OpenDICOMFile();
            DataContext = _viewModel;

            // ImageContainer の中に ImageViewer を配置する
            var imageContainer = (ContentControl)FindName("ImageContainer");
            imageContainer.Content = imageViewer;
        }
    }
}
