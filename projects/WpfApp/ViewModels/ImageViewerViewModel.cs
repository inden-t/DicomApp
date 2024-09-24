using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DicomApp.BloodVesselExtraction.ViewModels;
using DicomApp.CoreModels.Models;
using DicomApp.WpfUtilities.ViewModels;
using Reactive.Bindings;

namespace DicomApp.WpfApp.ViewModels
{
    public class ImageViewerViewModel : ViewModelBase
    {
        private readonly SelectionOverlayControlViewModel
            _overlayControlViewModel;

        private double _zoom = 1.0;
        private double _viewerWidth;
        private double _viewerHeight;

        private readonly Dictionary<int, WriteableBitmap> _bitmapCache = new();

        public ReactiveCollection<DICOMFile> DicomFiles { get; } = new();

        public ReactiveProperty<BitmapSource> BitmapSourceImage { get; } =
            new();

        public ReactiveProperty<int> MaximumScrollValue { get; } = new();
        public ReactiveProperty<int> SelectedFileIndex { get; } = new();

        public ReactiveProperty<string> PixelInfo { get; } = new();

        public double Zoom
        {
            get => _zoom;
            private set
            {
                _zoom = value;
                _overlayControlViewModel.Zoom = value;
            }
        }

        public double ViewerWidth
        {
            get => _viewerWidth;
            private set
            {
                _viewerWidth = value;
                _overlayControlViewModel.ViewerWidth = value;
            }
        }

        public double ViewerHeight
        {
            get => _viewerHeight;
            private set
            {
                _viewerHeight = value;
                _overlayControlViewModel.ViewerHeight = value;
            }
        }

        public ImageViewerViewModel(
            SelectionOverlayControlViewModel overlayControlViewModel)
        {
            _overlayControlViewModel = overlayControlViewModel;
            _overlayControlViewModel.SliceIndex = SelectedFileIndex;
            _overlayControlViewModel.GetSliceImage = GetBitmapImage;

            DicomFiles.CollectionChanged += (sender, e) =>
            {
                MaximumScrollValue.Value = DicomFiles.Count - 1;
            };

            SelectedFileIndex.Subscribe(index => Render());
        }

        public void SetDicomFiles(IEnumerable<DICOMFile> dicomFiles)
        {
            DicomFiles.Clear();
            _bitmapCache.Clear();
            DicomFiles.AddRange(dicomFiles);
            SelectedFileIndex.Value = 0;
            SelectedFileIndex.ForceNotify();
        }

        public void SwitchImageByOffset(int offset)
        {
            if (DicomFiles.Count == 0) return;

            int newIndex = SelectedFileIndex.Value + offset;
            newIndex = Math.Max(0, Math.Min(newIndex, DicomFiles.Count - 1));
            SelectedFileIndex.Value = newIndex;
        }

        public bool SetZoomValue(double factor)
        {
            bool isZoomed = false;

            double newZoom = Zoom * factor;
            if ((Zoom < 1 && newZoom > 1) || (Zoom > 1 && newZoom < 1))
            {
                Zoom = 1;
                isZoomed = true;
            }
            else if (newZoom <= 10)
            {
                Zoom = newZoom;
                isZoomed = true;
            }

            Render();

            return isZoomed;
        }

        public void UpdateViewerSize(double width, double height)
        {
            ViewerWidth = width;
            ViewerHeight = height;
            Render();
        }

        public void Render()
        {
            int currentIndex = SelectedFileIndex.Value;
            if (currentIndex < 0 || currentIndex >= DicomFiles.Count) return;

            var bitmapImage = GetBitmapImage(currentIndex);
            if (bitmapImage == null) return;

            // 枠に対するサイズを計算
            // 枠からはみ出ないように枠サイズの小数を切り捨てる
            double scaleX = Math.Floor(ViewerWidth) / bitmapImage.PixelWidth;
            double scaleY = Math.Floor(ViewerHeight) / bitmapImage.PixelHeight;
            double scale = Math.Min(scaleX, scaleY);

            // 拡大倍率を適用
            var scaledBitmap = new TransformedBitmap(bitmapImage,
                new ScaleTransform(scale * Zoom, scale * Zoom));

            BitmapSourceImage.Value = scaledBitmap;

            // 選択領域の表示を更新
            _overlayControlViewModel.UpdateSelectedRegion();
        }

        public void UpdatePixelInfoOnMouseMove(double relativeX,
            double relativeY)
        {
            int currentIndex = SelectedFileIndex.Value;
            if (currentIndex < 0 || currentIndex >= DicomFiles.Count) return;

            var bitmapImage = GetBitmapImage(currentIndex);
            if (bitmapImage == null) return;

            int x = (int)(relativeX * bitmapImage.PixelWidth);
            int y = (int)(relativeY * bitmapImage.PixelHeight);

            if (x >= 0 && x < bitmapImage.PixelWidth && y >= 0 &&
                y < bitmapImage.PixelHeight)
            {
                byte[] pixels = new byte[4];
                bitmapImage.CopyPixels(new Int32Rect(x, y, 1, 1), pixels,
                    4, 0);
                byte blue = pixels[0];
                PixelInfo.Value = $"座標: ({x}, {y})\nピクセル値: {blue}";
            }
        }

        private WriteableBitmap? GetBitmapImage(int index)
        {
            if (index < 0 || index >= DicomFiles.Count) return null;

            if (!_bitmapCache.TryGetValue(index, out var bitmapImage))
            {
                var selectedFile = DicomFiles[index];
                var dicomImage = selectedFile.GetImage();
                var renderedImage = dicomImage.RenderImage();
                bitmapImage = renderedImage.As<WriteableBitmap>();
                _bitmapCache[index] = bitmapImage;
            }

            return bitmapImage;
        }
    }
}
