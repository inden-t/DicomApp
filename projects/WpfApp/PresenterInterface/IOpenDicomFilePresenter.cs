using System;
using DicomApp.CoreModels.Models;

namespace DicomApp.PresenterInterface
{
    public interface IOpenDicomFilePresenter
    {
        void UpdateDisplayedImage(IEnumerable<DICOMFile> dicomFiles);
    }
}
