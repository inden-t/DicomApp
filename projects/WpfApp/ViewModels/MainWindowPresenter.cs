using System;
using System.Collections.ObjectModel;
using DicomApp.UseCases;

namespace DicomApp.ViewModels
{
    class MainWindowPresenter : IMainWindowPresenter
    {
        private readonly MainWindowViewModel _mainWindowViewModel;

        public MainWindowPresenter(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
        }

        public void UpdateDisplayedImage(IEnumerable<DICOMFile> dicomFiles)
        {
            _mainWindowViewModel.DicomFiles.Clear();
            _mainWindowViewModel.DicomFiles.AddRange(dicomFiles);
            _mainWindowViewModel.SelectedIndex.Value = 0;
            _mainWindowViewModel.SelectedIndex.ForceNotify();
        }
    }
}
