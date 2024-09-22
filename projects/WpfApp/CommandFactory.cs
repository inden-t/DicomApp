using System;
using DicomApp.BloodVesselExtraction.UseCases;
using DicomApp.BloodVesselExtraction.ViewModels;
using DicomApp.MainUseCases.UseCases;
using DicomApp.UseCases;
using DicomApp.ViewModels;

namespace DicomApp
{
    class CommandFactory
    {
        public CommandFactory(MainWindowViewModel mainWindowViewModel,
            OpenDicomFileUseCase openDicomFileUseCase,
            GeneratePointCloudUseCase
                generatePointCloudUseCase,
            GenerateSurfaceModelUseCase
                generateSurfaceModelUseCase,
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
                generatePointCloudUseCase, generateSurfaceModelUseCase,
                generateSurfaceModelLinearInterpolationUseCase,
                bloodVesselExtractionUseCase, select3DBloodVesselRegionUseCase);

            bloodVesselExtractionRibbonTabViewModel.InitializeDependencies(
                bloodVesselExtractionUseCase, manageBloodVesselRegionUseCase);

            selectionOverlayControlViewModel.InitializeDependencies(
                select3DBloodVesselRegionUseCase);
        }
    }
}
