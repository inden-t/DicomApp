using System;
using DicomApp.UseCases;
using DicomApp.ViewModels;

namespace DicomApp
{
    class CommandFactory
    {
        public CommandFactory(MainWindowViewModel mainWindowViewModel,
            OpenDicomFileUseCase openDicomFileUseCase,
            MakeBloodVesselPointCloud3DUseCase
                makeBloodVesselPointCloud3DUseCase)
        {
            mainWindowViewModel.OpenDicomFileCommand.Subscribe(async _ =>
                await openDicomFileUseCase.ExecuteAsync());
            mainWindowViewModel.OpenDicomFolderCommand.Subscribe(async _ =>
                await openDicomFileUseCase.ExecuteFolderAsync());

            mainWindowViewModel.MakeBloodVesselPointCloud3DCommand.Subscribe(
                async () =>
                    await makeBloodVesselPointCloud3DUseCase.ExecuteAsync());
        }
    }
}
