namespace DicomApp.CoreModels.Models
{
    public class FileManager
    {
        private readonly IImageCaches _imageCaches;

        public List<DICOMFile> DicomFiles { get; } = new();

        public FileManager(IImageCaches imageCaches)
        {
            _imageCaches = imageCaches;
        }

        public void ClearFiles()
        {
            DicomFiles.Clear();
            _imageCaches.Clear();
        }

        public void AddFile(DICOMFile file)
        {
            DicomFiles.Add(file);
            _imageCaches.AddFile(file);
        }
    }
}
