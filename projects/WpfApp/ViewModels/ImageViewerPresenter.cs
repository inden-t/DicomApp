using System;
using DicomApp.Models;
using DicomApp.UseCases;

namespace DicomApp.ViewModels
{
    public class ImageViewerPresenter : IImageViewerPresenter
    {
        private readonly ImageViewerViewModel _imageViewerViewModel;

        public ImageViewerPresenter(ImageViewerViewModel imageViewerViewModel)
        {
            _imageViewerViewModel = imageViewerViewModel;
        }

        public void RenderImage()
        {
            _imageViewerViewModel.Render();
        }

        public void SetSelectedRegion(BloodVessel3DRegion selectedRegion)
        {
            _imageViewerViewModel.SetSelectedRegion(selectedRegion);
        }

        public void SetSelectionModeActive(bool isActive)
        {
            _imageViewerViewModel.IsSelectionModeActive.Value = isActive;
        }

        public void ResetSelectionMode()
        {
            _imageViewerViewModel.CurrentSelectionMode.Value =
                SelectionMode.None;
            _imageViewerViewModel.IsSelectionVisible.Value = true;
        }
    }
}
