using DicomApp.Models;
using DicomApp.UseCases;

public class BloodVesselExtractionUseCase
{
    private readonly BloodVessel3DRegionSelector _regionSelector;
    private readonly BloodVesselSurfaceModelGenerator _modelGenerator;

    public BloodVesselExtractionUseCase(
        BloodVessel3DRegionSelector regionSelector,
        BloodVesselSurfaceModelGenerator modelGenerator)
    {
        _regionSelector = regionSelector;
        _modelGenerator = modelGenerator;
    }

    public async Task<SurfaceModel> ExtractBloodVesselAsync(
        //int threshold
        //,
        //IProgress<int> progress
    )
    {
        int threshold = 200;
        var region = _regionSelector.GetSelectedRegion();
        return await _modelGenerator.GenerateModelAsync(region, threshold
            //,
            //progress
        );
    }
}
