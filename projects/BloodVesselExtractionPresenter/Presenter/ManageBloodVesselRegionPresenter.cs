using System;
using DicomApp.Models;
using DicomApp.PresenterInterface;
using DicomApp.ViewModels;

namespace DicomApp.Presenter
{
    public class
        ManageBloodVesselRegionPresenter : IManageBloodVesselRegionPresenter
    {
        private readonly SelectionOverlayControlViewModel
            _overlayControlViewModel;

        public ManageBloodVesselRegionPresenter(
            SelectionOverlayControlViewModel overlayControlViewModel)
        {
            _overlayControlViewModel = overlayControlViewModel;
        }

        public void InitializeRegionSelection(
            BloodVessel3DRegion selectedRegion)
        {
            _overlayControlViewModel.IsSelectionModeActive.Value = false;
            _overlayControlViewModel.CurrentSelectionMode.Value =
                SelectionMode.None;
            _overlayControlViewModel.IsVisible.Value = true;
            _overlayControlViewModel.SetSelectedRegion(selectedRegion);
        }

        public void UpdateSelectedRegion(BloodVessel3DRegion selectedRegion)
        {
            _overlayControlViewModel.SetSelectedRegion(selectedRegion);
        }
    }
}
