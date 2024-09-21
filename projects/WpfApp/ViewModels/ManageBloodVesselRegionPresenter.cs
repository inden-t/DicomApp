using DicomApp.Models;
using System;

namespace DicomApp.ViewModels
{
    public interface IManageBloodVesselRegionPresenter
    {
        void InitializeRegionSelection(BloodVessel3DRegion selectedRegion);
        void UpdateSelectedRegion(BloodVessel3DRegion selectedRegion);
    }
}
