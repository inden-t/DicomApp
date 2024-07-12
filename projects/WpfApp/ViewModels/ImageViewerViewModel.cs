using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using FellowOakDicom.Imaging;
using Reactive.Bindings;

namespace DicomApp.ViewModels
{
    public class ImageViewerViewModel : ViewModelBase
    {
        private DicomImage _image;
        private double _zoom = 1.0;
        private double _rotation = 0.0;

        public ReactiveProperty<BitmapSource> BitmapSourceImage { get; } =
            new();

        public ReactiveProperty<int> MaximumScrollValue { get; } = new();
        public ReactiveProperty<int> ScrollValue { get; } = new();

        public ReactiveCommand<int> SwitchImageByIndexCommand { get; } = new();

        public ReactiveCommand<int> SwitchImageByOffsetCommand { get; } = new();

        public double ViewerWidth { get; private set; }
        public double ViewerHeight { get; private set; }

        public ImageViewerViewModel()
        {
            ScrollValue.Subscribe(value =>
                SwitchImageByIndexCommand.Execute(value));
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

        public void Zoom(double factor)
        {
            _zoom *= factor;
            Render();
        }

        public void Rotate(double angle)
        {
            _rotation += angle;
            Render();
        }

        public void UpdateViewerSize(double width, double height)
        {
            ViewerWidth = width;
            ViewerHeight = height;
            Render();
        }

        private void Render()
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

            //// 回転を適用
            //var rotatedBitmap = new TransformedBitmap(scaledBitmap,
            //    new RotateTransform(_rotation));

            BitmapSourceImage.Value = scaledBitmap;
        }
    }
}
