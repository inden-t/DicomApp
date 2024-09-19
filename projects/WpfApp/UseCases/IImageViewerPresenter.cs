using System;

namespace DicomApp.UseCases
{
    public interface IImageViewerPresenter
    {
        void RenderImage();
        void SetSelectionModeActive(bool isActive);
    }
}
