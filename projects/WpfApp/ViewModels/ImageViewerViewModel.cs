using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using DicomApp.Models;
using DicomApp.UseCases;
using FellowOakDicom.Imaging;
using Reactive.Bindings;

namespace DicomApp.ViewModels
{
    public class ImageViewerViewModel : ViewModelBase
    {
        private readonly SelectionOverlayControlViewModel
            _overlayControlViewModel;

        private DicomImage _image;
        private double _zoom = 1.0;

        public SelectionOverlayControlViewModel
            SelectionOverlayControlViewModel => _overlayControlViewModel;

        public ReactiveProperty<BitmapSource> BitmapSourceImage { get; } =
            new();

        public ReactiveProperty<int> MaximumScrollValue { get; } = new();
        public ReactiveProperty<int> ScrollValue { get; } = new();

        public ReactiveCommand<int> SwitchImageByIndexCommand { get; } = new();

        public ReactiveCommand<int> SwitchImageByOffsetCommand { get; } = new();

        public ReactiveProperty<bool> IsSelectionModeActive { get; } =
            new(false);

        public double ViewerWidth { get; private set; }
        public double ViewerHeight { get; private set; }

        private BloodVessel3DRegion _selectedRegion = new();

        private Select3DBloodVesselRegionUseCase
            _select3DBloodVesselRegionUseCase;

        public ImageViewerViewModel(
            SelectionOverlayControlViewModel overlayControlViewModel)
        {
            _overlayControlViewModel = overlayControlViewModel;

            ScrollValue.Subscribe(value =>
                SwitchImageByIndexCommand.Execute(value));
        }

        public void InitializeDependencies(
            Select3DBloodVesselRegionUseCase select3DBloodVesselRegionUseCase)
        {
            _select3DBloodVesselRegionUseCase =
                select3DBloodVesselRegionUseCase;
        }

        public void SwitchImageByOffset(int offset)
        {
            // MainWindowViewModelに画像切り替えを通知
            SwitchImageByOffsetCommand.Execute(offset);
        }

        public void SetImage(DicomImage image)
        {
            _image = image;
            Render();
        }

        public void SetMaximumScrollValue(int value)
        {
            MaximumScrollValue.Value = value;
        }

        public bool Zoom(double factor)

        {
            bool isZoomed = false;

            double newZoom = _zoom * factor;
            if ((_zoom < 1 && newZoom > 1) || (_zoom > 1 && newZoom < 1))
            {
                _zoom = 1;
                isZoomed = true;
            }
            else if (newZoom <= 10)
            {
                _zoom = newZoom;
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
                new ScaleTransform(scale * _zoom, scale * _zoom));

            BitmapSourceImage.Value = scaledBitmap;

            // 選択領域の表示を更新
            _overlayControlViewModel.UpdateSelectedRegion(_selectedRegion,
                _image, ViewerWidth, ViewerHeight, ScrollValue.Value, _zoom);
        }

        public void OnClick(double relativeX, double relativeY)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            var renderedImage = _image.RenderImage();
            var bitmapImage = renderedImage.As<WriteableBitmap>();

            Point3D seedPoint = new Point3D(relativeX * bitmapImage.PixelWidth,
                relativeY * bitmapImage.PixelHeight,
                ScrollValue.Value);

            if (_overlayControlViewModel.CurrentSelectionMode.Value ==
                SelectionMode.Fill3DSelection)
            {
                _select3DBloodVesselRegionUseCase.Execute3DFillSelection(
                    seedPoint);
            }
            else if (_overlayControlViewModel.CurrentSelectionMode.Value ==
                     SelectionMode.Clear3DFillSelection)
            {
                _select3DBloodVesselRegionUseCase.Clear3DFillSelection(
                    seedPoint);
            }
            else if (_overlayControlViewModel.CurrentSelectionMode.Value ==
                     SelectionMode.Fill2DSelection)
            {
                _select3DBloodVesselRegionUseCase.Execute2DFillSelection(
                    seedPoint);
            }
            else if (_overlayControlViewModel.CurrentSelectionMode.Value ==
                     SelectionMode.ClearFill2DSelection)
            {
                _select3DBloodVesselRegionUseCase.Clear2DFillSelection(
                    seedPoint);
            }

            Mouse.OverrideCursor = null;
        }

        public void SetSelectedRegion(BloodVessel3DRegion selectedRegion)
        {
            _selectedRegion = selectedRegion;
            _overlayControlViewModel.UpdateSelectedRegion(_selectedRegion,
                _image, ViewerWidth, ViewerHeight, ScrollValue.Value, _zoom);
        }
    }
}
