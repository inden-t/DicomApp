using System.Windows.Media.Imaging;
using DicomApp.Models;

namespace DicomApp.ViewModels
{
    public class ImageCache : IImageCache
    {
        private Dictionary<string, WriteableBitmap> _cache = new();

        public void AddFile(DICOMFile file)
        {
            string key = file.FilePath;
            var _image = file.GetImage();
            var renderedImage = _image.RenderImage();
            var writeableBitmap = renderedImage.As<WriteableBitmap>();
            _cache[key] = writeableBitmap;
        }

        public void Clear()
        {
            _cache.Clear();
        }
    }
}
