using System;
using DicomApp.UseCases;
using DicomApp.ViewModels;

namespace DicomApp
{
    class CommandFactory
    {
        public CommandFactory(MainWindowViewModel mainWindowViewModel,
            OpenDicomFileUseCase openDicomFileUseCase)
        {
            mainWindowViewModel.OpenDicomFileCommand.Subscribe(_ =>
                openDicomFileUseCase.Execute());
            mainWindowViewModel.OpenDicomFolderCommand.Subscribe(_ =>
                openDicomFileUseCase.ExecuteFolder());
        }
    }
}
