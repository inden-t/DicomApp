using System;
using DicomApp.Models;
using DicomApp.PresenterInterface;
using DicomApp.ViewModels;

namespace DicomApp.Presenter
{
    public class
        ManageBloodVesselRegionPresenter : IManageBloodVesselRegionPresenter
    {
        private readonly ImageViewerViewModel _imageViewerViewModel;

        private readonly SelectionOverlayControlViewModel
            _overlayControlViewModel;

        public ManageBloodVesselRegionPresenter(
            ImageViewerViewModel imageViewerViewModel,
            SelectionOverlayControlViewModel overlayControlViewModel)
        {
            _imageViewerViewModel = imageViewerViewModel;
            _overlayControlViewModel = overlayControlViewModel;
        }

        public void InitializeRegionSelection(
            BloodVessel3DRegion selectedRegion)
        {
            _overlayControlViewModel.IsSelectionModeActive.Value = false;
            _overlayControlViewModel.CurrentSelectionMode.Value =
                SelectionMode.None;
            _overlayControlViewModel.IsVisible.Value = true;
            _imageViewerViewModel.SetSelectedRegion(selectedRegion);
        }

        public void UpdateSelectedRegion(BloodVessel3DRegion selectedRegion)
        {
            _imageViewerViewModel.SetSelectedRegion(selectedRegion);
        }
    }
}
