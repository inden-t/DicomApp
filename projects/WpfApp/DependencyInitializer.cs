using DicomApp.BloodVesselExtraction.UseCases;
using DicomApp.BloodVesselExtraction.ViewModels;
using DicomApp.MainUseCases.UseCases;
using DicomApp.WpfApp.UseCases;
using DicomApp.WpfApp.ViewModels;

namespace DicomApp.WpfApp
{
    class DependencyInitializer
    {
        public DependencyInitializer(MainWindowViewModel mainWindowViewModel,
            OpenDicomFileUseCase openDicomFileUseCase,
            LoadModel3dUseCase loadModel3dUseCase,
            GeneratePointCloudUseCase generatePointCloudUseCase,
            GenerateSurfaceModelUseCase generateSurfaceModelUseCase,
            GenerateSurfaceModelLinearInterpolationUseCase
                generateSurfaceModelLinearInterpolationUseCase,
            BloodVesselExtractionRibbonTabViewModel
                bloodVesselExtractionRibbonTabViewModel,
            BloodVesselExtractionUseCase bloodVesselExtractionUseCase,
            Select3DBloodVesselRegionUseCase select3DBloodVesselRegionUseCase,
            ManageBloodVesselRegionUseCase manageBloodVesselRegionUseCase,
            SelectionOverlayControlViewModel selectionOverlayControlViewModel)
        {
            mainWindowViewModel.InitializeDependencies(openDicomFileUseCase,
                loadModel3dUseCase, generatePointCloudUseCase,
                generateSurfaceModelUseCase,
                generateSurfaceModelLinearInterpolationUseCase,
                bloodVesselExtractionUseCase, select3DBloodVesselRegionUseCase);

            bloodVesselExtractionRibbonTabViewModel.InitializeDependencies(
                bloodVesselExtractionUseCase, manageBloodVesselRegionUseCase);

            selectionOverlayControlViewModel.InitializeDependencies(
                select3DBloodVesselRegionUseCase);
        }
    }
}
