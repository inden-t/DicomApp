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
        private DicomImage _image;
        private double _zoom = 1.0;

        public ReactiveProperty<BitmapSource> BitmapSourceImage { get; } =
            new();

        public ReactiveProperty<BitmapSource> OverlayImageSource { get; } =
            new();

        public ReactiveProperty<int> MaximumScrollValue { get; } = new();
        public ReactiveProperty<int> ScrollValue { get; } = new();

        public ReactiveCommand<int> SwitchImageByIndexCommand { get; } = new();

        public ReactiveCommand<int> SwitchImageByOffsetCommand { get; } = new();

        public double ViewerWidth { get; private set; }
        public double ViewerHeight { get; private set; }

        private readonly BloodVessel3DRegionSelector _regionSelector;
        private readonly IProgressWindowFactory _progressWindowFactory;

        public ImageViewerViewModel(BloodVessel3DRegionSelector regionSelector,
            IProgressWindowFactory progressWindowFactory)
        {
            _regionSelector = regionSelector;
            _progressWindowFactory = progressWindowFactory;

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

            BitmapSourceImage.Value = scaledBitmap;
        }

        public void StartBloodVesselSelectionMode()
        {
            // 血管領域選択モードを開始
        }

        public async Task Select3DRegion(double relativeX, double relativeY)
        {
            var renderedImage = _image.RenderImage();
            var bitmapImage = renderedImage.As<WriteableBitmap>();

            Point3D seedPoint = new Point3D(relativeX * bitmapImage.PixelWidth,
                relativeY * bitmapImage.PixelHeight,
                ScrollValue.Value);

            IProgressWindow progressWindow = _progressWindowFactory.Create();
            progressWindow.SetWindowTitle("モデル生成中");
            progressWindow.Start();
            progressWindow.SetStatusText("3次元塗りつぶし選択を実行中...");

            var progress = new Progress<(int value, int pointNum)>(data =>
            {
                progressWindow.SetStatusText(
                    $"3次元塗りつぶし選択を実行中...\n" +
                    $"進捗: {data.value}%\n" +
                    $"点の個数: {data.pointNum}個");
                progressWindow.SetProgress(data.value);

                // ここでUIの更新などを行うことができます
            });

            int threshold = 200; // しきい値は適切な値に変更してください
            await Task.Run(() =>
                _regionSelector.Select3DRegion(seedPoint, threshold, progress));

            progressWindow.End();

            // 選択領域の表示を更新
            UpdateSelectedRegion();
        }

        private void UpdateSelectedRegion()
        {
            // 選択された3D領域を表示するための処理を実装
            // 例えば、CanvasOverlayにグラフィックを描画するなど
        }

        public void EditRegion(Point3D point, bool isAdd)
        {
            _regionSelector.EditRegion(point, isAdd);
            // 選択領域の表示を更新
        }
    }
}
