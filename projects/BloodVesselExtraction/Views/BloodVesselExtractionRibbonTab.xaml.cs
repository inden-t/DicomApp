using System.Windows.Controls.Ribbon;
using DicomApp.BloodVesselExtraction.ViewModels;

namespace DicomApp.BloodVesselExtraction.Views
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
