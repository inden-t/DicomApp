using System;
using DicomApp.Models;

namespace DicomApp.UseCases
{
    public interface IManageBloodVesselRegionPresenter
    {
        void InitializeRegionSelection(BloodVessel3DRegion selectedRegion);
        void UpdateSelectedRegion(BloodVessel3DRegion selectedRegion);
    }
}
