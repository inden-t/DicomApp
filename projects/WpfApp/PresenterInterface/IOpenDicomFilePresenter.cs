using DicomApp.CoreModels.Models;

namespace DicomApp.WpfApp.PresenterInterface
{
    public interface IOpenDicomFilePresenter
    {
        void UpdateDisplayedImage(IEnumerable<DICOMFile> dicomFiles);
    }
}
