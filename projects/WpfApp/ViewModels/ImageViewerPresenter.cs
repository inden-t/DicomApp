using System;
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
    }
}
