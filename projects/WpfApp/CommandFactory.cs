using System;
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
            ImageViewerViewModel imageViewerViewModel)
        {
            mainWindowViewModel.InitializeDependencies(openDicomFileUseCase,
                generatePointCloudUseCase, generateSurfaceModelUseCase,
                generateSurfaceModelLinearInterpolationUseCase);

            bloodVesselExtractionRibbonTabViewModel.InitializeDependencies(
                bloodVesselExtractionUseCase, select3DBloodVesselRegionUseCase,
                manageBloodVesselRegionUseCase);

            imageViewerViewModel.InitializeDependencies(
                select3DBloodVesselRegionUseCase);
        }
    }
}
