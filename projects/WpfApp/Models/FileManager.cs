using System;
using Reactive.Bindings;

namespace DicomApp.Models
{
    public class FileManager
    {
        private readonly IImageCaches _imageCaches;

        public ReactiveCollection<DICOMFile> DicomFiles { get; } = new();

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
