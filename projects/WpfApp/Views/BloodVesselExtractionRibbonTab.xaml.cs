using System.Windows.Controls.Ribbon;
using DicomApp.ViewModels;

namespace DicomApp.Views
{
    public partial class BloodVesselExtractionRibbonTab : RibbonTab
    {
        private readonly BloodVesselExtractionRibbonTabViewModel _viewModel;

        public BloodVesselExtractionRibbonTab(
            BloodVesselExtractionRibbonTabViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }
    }
}
