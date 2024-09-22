using DicomApp.BloodVesselExtraction.Models;

namespace DicomApp.BloodVesselExtraction.PresenterInterface
{
    public interface IManageBloodVesselRegionPresenter
    {
        void InitializeRegionSelection(BloodVessel3DRegion selectedRegion);
        void UpdateSelectedRegion(BloodVessel3DRegion selectedRegion);
    }
}
