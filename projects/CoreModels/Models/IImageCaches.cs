namespace DicomApp.CoreModels.Models
{
    public interface IImageCaches
    {
        void AddFile(DICOMFile file);
        void Clear();
    }
}
