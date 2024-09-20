using System;
using DicomApp.Models;

namespace DicomApp.UseCases
{
    public interface IImageViewerPresenter
    {
        void RenderImage();
        void SetSelectedRegion(BloodVessel3DRegion selectedRegion);
        void SetSelectionModeActive(bool isActive);
    }
}
