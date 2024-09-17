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
            DisplaySurfaceModelUseCase
                displaySurfaceModelUseCase,
            DisplaySurfaceModelLinearInterpolationUseCase
                displaySurfaceModelLinearInterpolationUseCase)
        {
            mainWindowViewModel.OpenDicomFileCommand.Subscribe(async _ =>
                await openDicomFileUseCase.ExecuteAsync());
            mainWindowViewModel.OpenDicomFolderCommand.Subscribe(async _ =>
                await openDicomFileUseCase.ExecuteFolderAsync());

            mainWindowViewModel.DisplayPointCloud3dCommand.Subscribe(
                async () =>
                    await generatePointCloudUseCase.ExecuteAsync());

            mainWindowViewModel.DisplaySurfaceModelCommand.Subscribe(
                async () =>
                    await displaySurfaceModelUseCase.ExecuteAsync());

            mainWindowViewModel.DisplaySurfaceModelLinearInterpolationCommand
                .Subscribe(async () =>
                    await displaySurfaceModelLinearInterpolationUseCase
                        .ExecuteAsync());
        }
    }
}
