using System;
using System.Collections.ObjectModel;
using DicomApp.UseCases;

namespace DicomApp.ViewModels
{
    class MainWindowPresenter : IMainWindowPresenter
    {
        private readonly MainWindowViewModel _mainWindowViewModel;
        private readonly ImageViewerViewModel _imageViewerViewModel;

        public MainWindowPresenter(MainWindowViewModel mainWindowViewModel,
            ImageViewerViewModel imageViewerViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
            _imageViewerViewModel = imageViewerViewModel;
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
