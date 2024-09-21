namespace DicomApp.Models
{
    public interface IImageCaches
    {
        void AddFile(DICOMFile file);
        void Clear();
    }
}
