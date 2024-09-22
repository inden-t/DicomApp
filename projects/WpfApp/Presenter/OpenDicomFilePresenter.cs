using System.Collections.ObjectModel;
using DicomApp.CoreModels.Models;
using DicomApp.WpfApp.PresenterInterface;
using DicomApp.WpfApp.ViewModels;

namespace DicomApp.WpfApp.Presenter
{
    class OpenDicomFilePresenter : IOpenDicomFilePresenter
    {
        private readonly MainWindowViewModel _mainWindowViewModel;

        public OpenDicomFilePresenter(MainWindowViewModel mainWindowViewModel)
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
