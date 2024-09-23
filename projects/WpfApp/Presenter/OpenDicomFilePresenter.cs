using System.Collections.ObjectModel;
using DicomApp.CoreModels.Models;
using DicomApp.WpfApp.PresenterInterface;
using DicomApp.WpfApp.ViewModels;

namespace DicomApp.WpfApp.Presenter
{
    class OpenDicomFilePresenter : IOpenDicomFilePresenter
    {
        private readonly ImageViewerViewModel _imageViewerViewModel;

        public OpenDicomFilePresenter(ImageViewerViewModel imageViewerViewModel)
        {
            _imageViewerViewModel = imageViewerViewModel;
        }

        public void UpdateDisplayedImage(IEnumerable<DICOMFile> dicomFiles)
        {
            _imageViewerViewModel.DicomFiles.Clear();
            _imageViewerViewModel.DicomFiles.AddRange(dicomFiles);
            _imageViewerViewModel.SelectedFileIndex.Value = 0;
            _imageViewerViewModel.SelectedFileIndex.ForceNotify();
        }
    }
}
