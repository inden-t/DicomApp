using System.Windows;
using System.Windows.Media;
using DicomApp.Models;
using System.Windows.Media.Imaging;
using Reactive.Bindings;
using FellowOakDicom.Imaging;

namespace DicomApp.ViewModels
{
    public class SelectionOverlayControlViewModel : ViewModelBase
    {
        public ReactiveProperty<ImageSource> OverlaySource { get; }
        public ReactiveProperty<bool> IsVisible { get; }

        public SelectionOverlayControlViewModel()
        {
            OverlaySource = new ReactiveProperty<ImageSource>();
            IsVisible = new ReactiveProperty<bool>(true);
        }

        public void UpdateSelectedRegion(BloodVessel3DRegion _selectedRegion,
            DicomImage _image, double ViewerWidth, double ViewerHeight, int z,
            double _zoom)
        {
            if (_image == null || _selectedRegion == null)
                return;

            var renderedImage = _image.RenderImage();
            var bitmapImage = renderedImage.As<WriteableBitmap>();

            // 新しいWriteableBitmapを作成し、透明な背景で初期化
            var overlayBitmap = new WriteableBitmap(bitmapImage.PixelWidth,
                bitmapImage.PixelHeight, bitmapImage.DpiX, bitmapImage.DpiY,
                PixelFormats.Bgra32, null);
            var stride = overlayBitmap.PixelWidth * 4;
            var pixels = new byte[overlayBitmap.PixelHeight * stride];


            // 選択された領域を描画
            foreach (var point in _selectedRegion.SelectedVoxels)
            {
                if (point.Z == z) // 現在のスライスのみ描画
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
                new ScaleTransform(scale * _zoom, scale * _zoom));

            // OverlayImageSourceを更新
            OverlaySource.Value = scaledBitmap;
        }
    }
}
