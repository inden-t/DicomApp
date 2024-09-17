using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using DicomApp.ViewModels;

namespace DicomApp.Views
{
    public partial class MainWindow : RibbonWindow
    {
        private readonly MainWindowViewModel _viewModel;

        public MainWindow(MainWindowViewModel viewModel,
            ImageViewer imageViewer,
            BloodVesselExtractionRibbonTab bloodVesselExtractionRibbonTab)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;

            // ImageContainer の中に ImageViewer を配置する
            var imageContainer = (ContentControl)FindName("ImageContainer");
            imageContainer.Content = imageViewer;

            // Ribbon の中に BloodVesselExtractionRibbonTab を配置する
            var ribbon = (Ribbon)FindName("Ribbon");
            ribbon.Items.Add(bloodVesselExtractionRibbonTab);
        }
    }
}
