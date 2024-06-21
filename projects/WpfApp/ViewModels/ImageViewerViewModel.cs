using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using FellowOakDicom.Imaging;
using Reactive.Bindings;

namespace DICOMViewer.ViewModels
{
    public class ImageViewerViewModel : ViewModelBase
    {
        private DicomImage _image;
        private double _zoom = 1.0;
        private double _rotation = 0.0;
        private Point _panOffset = new Point(0, 0);

        public ReactiveProperty<BitmapSource> BitmapSourceImage { get; } =
            new();

        public ImageViewerViewModel()
        {
        }

        public void SetImage(DicomImage image)
        {
            _image = image;
            Render();
        }

        public void Zoom(double factor)
        {
            _zoom *= factor;
            Render();
        }

        public void Pan(double x, double y)
        {
            _panOffset.X += x;
            _panOffset.Y += y;
            Render();
        }

        public void Rotate(double angle)
        {
            _rotation += angle;
            Render();
        }

        private void Render()
        {
            if (_image == null)
                return;

            // 画像を描画
            _image.Scale = _zoom;
            var renderedImage = _image.RenderImage();
            BitmapSourceImage.Value = renderedImage.As<WriteableBitmap>();
        }

        private void UserControl_MouseWheel(object sender,
            MouseWheelEventArgs e)
        {
            Zoom(Math.Pow(1.1, e.Delta / 120.0));
        }

        private void UserControl_MouseLeftButtonDown(object sender,
            MouseButtonEventArgs e)
        {
            //this.CaptureMouse();
        }

        private void UserControl_MouseLeftButtonUp(object sender,
            MouseButtonEventArgs e)
        {
            //this.ReleaseMouseCapture();
        }

        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            //if (this.IsMouseCaptured)
            //{
            //    var delta = e.GetPosition(this) - e.GetPosition(ImageHost);
            //    Pan(delta.X, delta.Y);
            //}
        }
    }
}
