﻿using System.Windows.Media.Imaging;
using DicomApp.CoreModels.Models;

namespace DicomApp.WpfApp.ViewModels
{
    public class ImageCaches : IImageCaches
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
