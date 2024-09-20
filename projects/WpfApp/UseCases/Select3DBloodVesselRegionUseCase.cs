using System.Windows.Media.Media3D;
using DicomApp.Models;

namespace DicomApp.UseCases
{
    public class Select3DBloodVesselRegionUseCase
    {
        private readonly BloodVessel3DRegionSelector _regionSelector;
        private readonly IImageViewerPresenter _imageViewerPresenter;

        private int _threshold = 220;

        public Select3DBloodVesselRegionUseCase(
            BloodVessel3DRegionSelector regionSelector,
            IImageViewerPresenter imageViewerPresenter)
        {
            _regionSelector = regionSelector;
            _imageViewerPresenter = imageViewerPresenter;
        }

        public void StartSelection(int threshold)
        {
            _threshold = threshold;
            _regionSelector.PreRenderImages();
        }

        public void Execute3DFillSelection(Point3D seedPoint)
        {
            _regionSelector.Select3DRegion(seedPoint, _threshold);
            UpdateSelectedRegion();
        }

        public void Clear3DFillSelection(Point3D seedPoint)
        {
            _regionSelector.Clear3DRegion(seedPoint);
            UpdateSelectedRegion();
        }

        public void Execute2DFillSelection(Point3D seedPoint)
        {
            _regionSelector.Select2DRegion(seedPoint, _threshold);
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
