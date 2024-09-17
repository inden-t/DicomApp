public class BloodVesselExtractionUseCase
{
    private readonly BloodVessel3DRegionSelector _regionSelector;
    private readonly BloodVesselSurfaceModelGenerator _modelGenerator;

    public BloodVesselExtractionUseCase(BloodVessel3DRegionSelector regionSelector, BloodVesselSurfaceModelGenerator modelGenerator)
    {
        _regionSelector = regionSelector;
        _modelGenerator = modelGenerator;
    }

    public async Task<SurfaceModel> ExtractBloodVesselAsync(int threshold, IProgress<int> progress)
    {
        var region = _regionSelector.GetSelectedRegion();
        return await _modelGenerator.GenerateModelAsync(region, threshold, progress);
    }
}
