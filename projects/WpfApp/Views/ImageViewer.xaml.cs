// cs
// 以下のように Image Viewer クラスを実装することができます。
// 
// ```csharp

using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using FellowOakDicom.Imaging;
using Reactive.Bindings;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using static System.Net.Mime.MediaTypeNames;
using Image = System.Windows.Controls.Image;
using Point = System.Windows.Point;

namespace DICOMViewer.Views
{
    public partial class ImageViewer : UserControl
    {
        private DicomImage _image;
        private double _zoom = 1.0;
        private double _rotation = 0.0;
        private Point _panOffset = new Point(0, 0);

        private Image _imageControl;
        private Canvas _imageCanvas;

        private BitmapSource _bitmapSourceImage;

        public ReactiveProperty<BitmapSource> BitmapSourceImage { get; } =
            new();

        public ImageViewer()
        {
            // GUIコンポーネントの初期化
            _imageControl = new Image();
            _imageCanvas = new Canvas();
            _imageCanvas.Children.Add(_imageControl);

            this.Content = _imageCanvas;
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

            // 画像の変換行列を作成
            var transform = new MatrixTransform();
            transform.Matrix = Matrix.Identity;
            transform.Matrix.ScaleAt(_zoom, _zoom, _image.Width / 2,
                _image.Height / 2);
            transform.Matrix.RotateAt(_rotation, _image.Width / 2,
                _image.Height / 2);
            transform.Matrix.Translate(_panOffset.X, _panOffset.Y);

            // 画像を描画
            //var bitmapSource = _image.RenderImage();
            //bitmapSource.Transform = transform;
            //_imageControl.Source = bitmapSource;

            //// キャンバスのサイズを調整
            //_imageCanvas.Width = bitmapSource.PixelWidth;
            //_imageCanvas.Height = bitmapSource.PixelHeight;

            _image.Scale = _zoom;
            var renderedImage = _image.RenderImage();
            _bitmapSourceImage = renderedImage.As<WriteableBitmap>();
            BitmapSourceImage.Value = _bitmapSourceImage;
        }
    }
}
// ```
// 
// このクラスでは、DICOM画像の表示と操作を行うことができます。
// 
// - `SetImage(DicomImage image)`: DICOM画像を設定します。
// - `Zoom(double factor)`: 画像をズームします。
// - `Pan(double x, double y)`: 画像をパンします。
// - `Rotate(double angle)`: 画像を回転します。
// - `Render()`: 画像を描画します。
// 
// このクラスは、ユーザー操作に応じて適切なメソッドを呼び出すことで、DICOM画像の表示と操作を行うことができます。
