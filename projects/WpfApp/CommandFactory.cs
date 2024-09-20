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
            ManageBloodVesselRegionUseCase manageBloodVesselRegionUseCase)
        {
            mainWindowViewModel.InitializeDependencies(openDicomFileUseCase,
                generatePointCloudUseCase, generateSurfaceModelUseCase,
                generateSurfaceModelLinearInterpolationUseCase);

            bloodVesselExtractionRibbonTabViewModel.InitializeDependencies(
                bloodVesselExtractionUseCase, manageBloodVesselRegionUseCase);
        }
    }
}
