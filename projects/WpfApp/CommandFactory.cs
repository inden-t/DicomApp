using System;
using DicomApp.UseCases;
using DicomApp.ViewModels;

namespace DicomApp
{
    class CommandFactory
    {
        public CommandFactory(MainWindowViewModel mainWindowViewModel,
            OpenDicomFileUseCase openDicomFileUseCase,
            MakeBloodVessel3DUseCase makeBloodVessel3DUseCase)
        {
            mainWindowViewModel.OpenDicomFileCommand.Subscribe(async _ =>
                await openDicomFileUseCase.ExecuteAsync());
            mainWindowViewModel.OpenDicomFolderCommand.Subscribe(async _ =>
                await openDicomFileUseCase.ExecuteFolderAsync());

            mainWindowViewModel.MakeBloodVessel3DCommand.Subscribe(async () =>
                await makeBloodVessel3DUseCase.ExecuteAsync());
        }
    }
}
