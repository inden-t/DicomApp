using System;

namespace DicomApp.UseCases
{
    public interface IOpenDicomFilePresenter
    {
        void UpdateDisplayedImage(IEnumerable<DICOMFile> dicomFiles);
    }
}
