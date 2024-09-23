﻿using System.Windows.Media;
using System.Windows.Media.Imaging;
using DicomApp.BloodVesselExtraction.Models;
using DicomApp.BloodVesselExtraction.ViewModels;
using DicomApp.CoreModels.Models;
using DicomApp.WpfUtilities.ViewModels;
using FellowOakDicom.Imaging;
using Reactive.Bindings;

namespace DicomApp.WpfApp.ViewModels
{
    public class ImageViewerViewModel : ViewModelBase
    {
        private readonly SelectionOverlayControlViewModel
            _overlayControlViewModel;

        private DicomImage _image;
        private double _zoom = 1.0;
        private double _viewerWidth;
        private double _viewerHeight;

        public ReactiveCollection<DICOMFile> DicomFiles { get; } = new();

        public ReactiveProperty<BitmapSource> BitmapSourceImage { get; } =
            new();

        public ReactiveProperty<int> MaximumScrollValue { get; } = new();
        public ReactiveProperty<int> ScrollValue { get; } = new();
        public ReactiveProperty<int> SelectedIndex { get; } = new(); //仮

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

        private BloodVessel3DRegion _selectedRegion = new();

        public ImageViewerViewModel(
            SelectionOverlayControlViewModel overlayControlViewModel)
        {
            _overlayControlViewModel = overlayControlViewModel;
            _overlayControlViewModel.ScrollValue = ScrollValue;

            DicomFiles.CollectionChanged += (sender, e) =>
            {
                MaximumScrollValue.Value = DicomFiles.Count - 1;
            };

            ScrollValue.Subscribe(value => SwitchImageByIndex(value));

            SelectedIndex.Subscribe(index => ChangeDisplayedImage(index));
        }

        private void SwitchImageByIndex(int index)
        {
            if (index >= 0 && index < DicomFiles.Count)
            {
                SelectedIndex.Value = index;
            }
        }

        public void SwitchImageByOffset(int offset)
        {
            if (DicomFiles.Count == 0) return;

            int newIndex = SelectedIndex.Value + offset;
            newIndex = Math.Max(0, Math.Min(newIndex, DicomFiles.Count - 1));
            SelectedIndex.Value = newIndex;
        }

        private void ChangeDisplayedImage(int index)
        {
            if (index < 0 || index >= DicomFiles.Count)
            {
                return;
            }

            var selectedFile = DicomFiles[index];
            if (selectedFile != null)
            {
                var image = selectedFile.GetImage();
                ScrollValue.Value = index;
                SetImage(image);
            }
        }


        public void SetImage(DicomImage image)
        {
            _image = image;
            _overlayControlViewModel.SetImage(image);
            Render();
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
            if (_image == null)
                return;

            // 画像を描画
            var renderedImage = _image.RenderImage();
            var bitmapImage = renderedImage.As<WriteableBitmap>();

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
    }
}
