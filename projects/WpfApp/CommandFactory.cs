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
            mainWindowViewModel.OpenDICOMFileCommand.Subscribe(_ =>
                openDicomFileUseCase.Execute());
        }
    }
}
