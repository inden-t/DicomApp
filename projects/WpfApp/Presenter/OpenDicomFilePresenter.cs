using System.Collections.ObjectModel;
using DicomApp.CoreModels.Models;
using DicomApp.WpfApp.PresenterInterface;
using DicomApp.WpfApp.ViewModels;

namespace DicomApp.WpfApp.Presenter
{
    class OpenDicomFilePresenter : IOpenDicomFilePresenter
    {
        private readonly MainWindowViewModel _mainWindowViewModel;
        private readonly ImageViewerViewModel _imageViewerViewModel;

        public OpenDicomFilePresenter(MainWindowViewModel mainWindowViewModel,
            ImageViewerViewModel imageViewerViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
            _imageViewerViewModel = imageViewerViewModel;
        }

        public void UpdateDisplayedImage(IEnumerable<DICOMFile> dicomFiles)
        {
            _imageViewerViewModel.DicomFiles.Clear();
            _imageViewerViewModel.DicomFiles.AddRange(dicomFiles);
            _mainWindowViewModel.SelectedIndex.Value = 0;
            _mainWindowViewModel.SelectedIndex.ForceNotify();
        }
    }
}
