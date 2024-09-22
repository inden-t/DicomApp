using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using DicomApp.BloodVesselExtraction.Models;
using DicomApp.BloodVesselExtraction.UseCases;
using DicomApp.WpfUtilities.ViewModels;
using FellowOakDicom.Imaging;
using Reactive.Bindings;

namespace DicomApp.BloodVesselExtraction.ViewModels
{
    public enum SelectionMode
    {
        None,
        Fill3DSelection,
        Clear3DFillSelection,
        Fill2DSelection,
        ClearFill2DSelection,
    }

    public class SelectionOverlayControlViewModel : ViewModelBase
    {
        private WriteableBitmap _bitmapImage;
        private BloodVessel3DRegion _selectedRegion = new();

        private Select3DBloodVesselRegionUseCase
            _select3DBloodVesselRegionUseCase;

        public ReactiveProperty<ImageSource> OverlaySource { get; } = new();
        public ReactiveProperty<bool> IsVisible { get; } = new(true);

        public ReactiveProperty<bool> IsSelectionModeActive { get; } =
            new(false);

        public ReactiveProperty<SelectionMode> CurrentSelectionMode { get; } =
            new(SelectionMode.None);

        public double ViewerWidth { get; set; }
        public double ViewerHeight { get; set; }
        public int ScrollValue { get; set; }
        public double Zoom { get; set; } = 1.0;

        public void InitializeDependencies(
            Select3DBloodVesselRegionUseCase select3DBloodVesselRegionUseCase)
        {
            _select3DBloodVesselRegionUseCase =
                select3DBloodVesselRegionUseCase;
        }

        public void OnClick(double relativeX, double relativeY)
        {
            Point3D seedPoint = new Point3D(relativeX * _bitmapImage.PixelWidth,
                relativeY * _bitmapImage.PixelHeight, ScrollValue);

            if (CurrentSelectionMode.Value ==
                SelectionMode.Fill3DSelection)
            {
                _select3DBloodVesselRegionUseCase.Execute3DFillSelection(
                    seedPoint);
            }
            else if (CurrentSelectionMode.Value ==
                     SelectionMode.Clear3DFillSelection)
            {
                _select3DBloodVesselRegionUseCase.Clear3DFillSelection(
                    seedPoint);
            }
            else if (CurrentSelectionMode.Value ==
                     SelectionMode.Fill2DSelection)
            {
                _select3DBloodVesselRegionUseCase.Execute2DFillSelection(
                    seedPoint);
            }
            else if (CurrentSelectionMode.Value ==
                     SelectionMode.ClearFill2DSelection)
            {
                _select3DBloodVesselRegionUseCase.Clear2DFillSelection(
                    seedPoint);
            }
        }

        public void SetImage(DicomImage image)
        {
            var renderedImage = image.RenderImage();
            var bitmapImage = renderedImage.As<WriteableBitmap>();
            _bitmapImage = bitmapImage;
        }

        public void SetSelectedRegion(BloodVessel3DRegion selectedRegion)
        {
            _selectedRegion = selectedRegion;
            UpdateSelectedRegion();
        }

        public void UpdateSelectedRegion()
        {
            if (_bitmapImage == null || _selectedRegion == null)
                return;

            // 新しいWriteableBitmapを作成し、透明な背景で初期化
            var overlayBitmap = new WriteableBitmap(_bitmapImage.PixelWidth,
                _bitmapImage.PixelHeight, _bitmapImage.DpiX, _bitmapImage.DpiY,
                PixelFormats.Bgra32, null);
            var stride = overlayBitmap.PixelWidth * 4;
            var pixels = new byte[overlayBitmap.PixelHeight * stride];


            // 選択された領域を描画
            foreach (var point in _selectedRegion.SelectedVoxels)
            {
                if (point.Z == ScrollValue) // 現在のスライスのみ描画
                {
                    int x = (int)point.X;
                    int y = (int)point.Y;
                    if (x >= 0 && x < overlayBitmap.PixelWidth && y >= 0 &&
                        y < overlayBitmap.PixelHeight)
                    {
                        int index = y * stride + x * 4;
                        pixels[index] = 0; // Blue
                        pixels[index + 1] = 0; // Green
                        pixels[index + 2] = 255; // Red
                        pixels[index + 3] = 128; // Alpha (半透明)
                    }
                }
            }

            // ピクセルデータをWriteableBitmapに書き込む
            overlayBitmap.WritePixels(
                new Int32Rect(0, 0, overlayBitmap.PixelWidth,
                    overlayBitmap.PixelHeight), pixels, stride, 0);

            // 枠に対するサイズを計算
            // 枠からはみ出ないように枠サイズの小数を切り捨てる
            double scaleX = Math.Floor(ViewerWidth) / overlayBitmap.PixelWidth;
            double scaleY =
                Math.Floor(ViewerHeight) / overlayBitmap.PixelHeight;
            double scale = Math.Min(scaleX, scaleY);

            // 拡大倍率を適用
            var scaledBitmap = new TransformedBitmap(overlayBitmap,
                new ScaleTransform(scale * Zoom, scale * Zoom));

            // OverlayImageSourceを更新
            OverlaySource.Value = scaledBitmap;
        }
    }
}
