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
            mainWindowViewModel.OpenDicomFileCommand.Subscribe(async _ =>
                await openDicomFileUseCase.ExecuteAsync());
            mainWindowViewModel.OpenDicomFolderCommand.Subscribe(async _ =>
                await openDicomFileUseCase.ExecuteFolderAsync());

            mainWindowViewModel.GeneratePointCloudCommand.Subscribe(
                async () =>
                    await generatePointCloudUseCase.ExecuteAsync());

            mainWindowViewModel.GenerateSurfaceModelCommand.Subscribe(
                async () =>
                    await generateSurfaceModelUseCase.ExecuteAsync());

            mainWindowViewModel.GenerateSurfaceModelLinearInterpolationCommand
                .Subscribe(async () =>
                    await generateSurfaceModelLinearInterpolationUseCase
                        .ExecuteAsync());

            bloodVesselExtractionRibbonTabViewModel.BloodVesselExtractionCommand
                .Subscribe(async () =>
                    await bloodVesselExtractionUseCase
                        .ExtractBloodVesselAsync());

            bloodVesselExtractionRibbonTabViewModel.SaveSelectionCommand
                .Subscribe(async () =>
                    await manageBloodVesselRegionUseCase
                        .SaveSelectedRegionAsync());
            bloodVesselExtractionRibbonTabViewModel.LoadSelectionCommand
                .Subscribe(async () =>
                    await manageBloodVesselRegionUseCase
                        .LoadSelectedRegionAsync());
            bloodVesselExtractionRibbonTabViewModel.ClearAllSelectionCommand
                .Subscribe(async () =>
                    await manageBloodVesselRegionUseCase
                        .ClearAllSelection());
        }
    }
}
