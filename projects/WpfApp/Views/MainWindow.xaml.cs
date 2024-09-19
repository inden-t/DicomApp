using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
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
            ImageContainer.Content = imageViewer;

            // Ribbon の中に BloodVesselExtractionRibbonTab を配置する
            Ribbon.Items.Add(bloodVesselExtractionRibbonTab);

            // Visibility を設定する
            SetupVisibilityBinding(bloodVesselExtractionRibbonTab);
        }

        private void SetupVisibilityBinding(FrameworkElement targetControl)
        {
            // バインディングを作成
            var binding =
                new Binding("ImageViewerViewModel.IsSelectionModeActive.Value")
                {
                    Source = DataContext,
                    Converter = new BooleanToVisibilityConverter()
                };

            // バインディングを適用
            targetControl.SetBinding(UIElement.VisibilityProperty, binding);
        }
    }
}
