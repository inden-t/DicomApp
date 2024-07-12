using System;

namespace DicomApp.UseCases
{
    public interface IMainWindowPresenter
    {
        void UpdateDisplayedImage(IEnumerable<DICOMFile> dicomFiles);
    }
}
