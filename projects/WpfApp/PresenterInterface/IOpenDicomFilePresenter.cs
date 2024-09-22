using System;

namespace DicomApp.PresenterInterface
{
    public interface IOpenDicomFilePresenter
    {
        void UpdateDisplayedImage(IEnumerable<DICOMFile> dicomFiles);
    }
}
