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
                generateSurfaceModelLinearInterpolationUseCase)
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
        }
    }
}
