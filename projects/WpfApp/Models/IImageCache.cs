namespace DicomApp.Models
{
    public interface IImageCache
    {
        void AddFile(DICOMFile file);
        void Clear();
    }
}
