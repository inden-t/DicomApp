using System.Windows.Media.Media3D;
using DicomApp.BloodVesselExtraction.Models;
using DicomApp.BloodVesselExtraction.PresenterInterface;

namespace DicomApp.BloodVesselExtraction.UseCases
{
    public class Select3DBloodVesselRegionUseCase
    {
        private readonly BloodVessel3DRegionSelector _regionSelector;

        private readonly IManageBloodVesselRegionPresenter
            _manageBloodVesselRegionPresenter;

        private int _threshold = 220;

        public Select3DBloodVesselRegionUseCase(
            BloodVessel3DRegionSelector regionSelector,
            IManageBloodVesselRegionPresenter manageBloodVesselRegionPresenter)
        {
            _regionSelector = regionSelector;
            _manageBloodVesselRegionPresenter =
                manageBloodVesselRegionPresenter;
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
            _manageBloodVesselRegionPresenter.UpdateSelectedRegion(
                selectedRegion);
        }
    }
}
