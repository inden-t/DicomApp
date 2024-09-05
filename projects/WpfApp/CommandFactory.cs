using System;
using DicomApp.UseCases;
using DicomApp.ViewModels;

namespace DicomApp
{
    class CommandFactory
    {
        public CommandFactory(MainWindowViewModel mainWindowViewModel,
            OpenDicomFileUseCase openDicomFileUseCase,
            DisplayPointCloud3dUseCase
                displayPointCloud3dUseCase,
            MakeBloodVesselSurfaceModelUseCase
                makeBloodVesselSurfaceModelUseCase)
        {
            mainWindowViewModel.OpenDicomFileCommand.Subscribe(async _ =>
                await openDicomFileUseCase.ExecuteAsync());
            mainWindowViewModel.OpenDicomFolderCommand.Subscribe(async _ =>
                await openDicomFileUseCase.ExecuteFolderAsync());

            mainWindowViewModel.MakeBloodVesselPointCloud3DCommand.Subscribe(
                async () =>
                    await displayPointCloud3dUseCase.ExecuteAsync());

            mainWindowViewModel.MakeBloodVesselSurfaceModelCommand.Subscribe(
                async () =>
                    await makeBloodVesselSurfaceModelUseCase.ExecuteAsync());
        }
    }
}
