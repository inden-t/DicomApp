using System.Windows.Media.Media3D;
using DicomApp.Models;

namespace DicomApp.UseCases
{
    public class Select3DBloodVesselRegionUseCase
    {
        private readonly BloodVessel3DRegionSelector _regionSelector;
        private readonly IImageViewerPresenter _imageViewerPresenter;

        public Select3DBloodVesselRegionUseCase(
            BloodVessel3DRegionSelector regionSelector,
            IImageViewerPresenter imageViewerPresenter)
        {
            _regionSelector = regionSelector;
            _imageViewerPresenter = imageViewerPresenter;
        }

        public void Execute3DFillSelection(Point3D seedPoint, int threshold)
        {
            _regionSelector.Select3DRegion(seedPoint, threshold);
            UpdateSelectedRegion();
        }

        public void Clear3DFillSelection(Point3D seedPoint)
        {
            _regionSelector.Clear3DRegion(seedPoint);
            UpdateSelectedRegion();
        }

        public void Execute2DFillSelection(Point3D seedPoint, int threshold)
        {
            _regionSelector.Select2DRegion(seedPoint, threshold);
            UpdateSelectedRegion();
        }

        public void Clear2DFillSelection(Point3D seedPoint)
        {
            _regionSelector.Clear2DRegion(seedPoint);
            UpdateSelectedRegion();
        }

        private void UpdateSelectedRegion()
        {
            var selectedRegion = _regionSelector.GetSelectedRegion();
            _imageViewerPresenter.SetSelectedRegion(selectedRegion);
        }
    }
}
