﻿using System;
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
    public enum SelectionMode
    {
        None,
        Fill3DSelection,
        Clear3DFillSelection,
        Fill2DSelection,
        ClearFill2DSelection,
    }

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

        public ReactiveProperty<SelectionMode> CurrentSelectionMode { get; } =
            new(SelectionMode.None);

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

            // 選択領域の表示を更新
            UpdateSelectedRegion();
        }

        public void StartBloodVesselSelectionMode()
        {
            // 血管領域選択モードを開始
        }

        public void OnClick(double relativeX, double relativeY)
        {
            if (CurrentSelectionMode.Value == SelectionMode.Fill3DSelection)
            {
                Select3DRegion(relativeX, relativeY);
            }
            else if (CurrentSelectionMode.Value ==
                     SelectionMode.Fill2DSelection)
            {
                Select2DRegion(relativeX, relativeY);
            }
            else if (CurrentSelectionMode.Value ==
                     SelectionMode.ClearFill2DSelection)
            {
                Clear2DRegion(relativeX, relativeY);
            }
            else if (CurrentSelectionMode.Value ==
                     SelectionMode.Clear3DFillSelection)
            {
                Clear3DRegion(relativeX, relativeY);
            }
        }

        public async Task Select3DRegion(double relativeX, double relativeY)
        {
            if (CurrentSelectionMode.Value != SelectionMode.Fill3DSelection)
                return;

            var renderedImage = _image.RenderImage();
            var bitmapImage = renderedImage.As<WriteableBitmap>();

            Point3D seedPoint = new Point3D(relativeX * bitmapImage.PixelWidth,
                relativeY * bitmapImage.PixelHeight,
                ScrollValue.Value);

            IProgressWindow progressWindow = _progressWindowFactory.Create();
            progressWindow.SetWindowTitle("モデル生成中");
            progressWindow.Start();
            progressWindow.SetStatusText("3次元塗りつぶし選択を実行中...");

            var progress = new Progress<(int value, string text)>(data =>
            {
                progressWindow.SetStatusText(data.text);
                progressWindow.SetProgress(data.value);
            });

            int threshold = 220; // しきい値は適切な値に変更してください
            await Task.Run(() =>
                _regionSelector.Select3DRegion(seedPoint, threshold));

            progressWindow.End();

            // 選択領域の表示を更新
            UpdateSelectedRegion();

            CurrentSelectionMode.Value = SelectionMode.None;
        }

        public async Task Clear3DRegion(double relativeX, double relativeY)
        {
            if (CurrentSelectionMode.Value !=
                SelectionMode.Clear3DFillSelection)
                return;

            var renderedImage = _image.RenderImage();
            var bitmapImage = renderedImage.As<WriteableBitmap>();

            Point3D seedPoint = new Point3D(relativeX * bitmapImage.PixelWidth,
                relativeY * bitmapImage.PixelHeight,
                ScrollValue.Value);

            IProgressWindow progressWindow = _progressWindowFactory.Create();
            progressWindow.SetWindowTitle("3D領域クリア中");
            progressWindow.Start();
            progressWindow.SetStatusText("3次元塗りつぶし選択を解除中...");

            var progress = new Progress<(int value, string text)>(data =>
            {
                progressWindow.SetStatusText(data.text);
                progressWindow.SetProgress(data.value);
            });

            await Task.Run(() => _regionSelector.Clear3DRegion(seedPoint));

            progressWindow.End();

            // 選択領域の表示を更新
            UpdateSelectedRegion();

            CurrentSelectionMode.Value = SelectionMode.None;
        }

        public void Clear2DRegion(double relativeX, double relativeY)
        {
            if (CurrentSelectionMode.Value !=
                SelectionMode.ClearFill2DSelection)
                return;

            var renderedImage = _image.RenderImage();
            var bitmapImage = renderedImage.As<WriteableBitmap>();

            Point3D seedPoint = new Point3D(relativeX * bitmapImage.PixelWidth,
                relativeY * bitmapImage.PixelHeight,
                ScrollValue.Value);

            _regionSelector.Clear2DRegion(seedPoint);

            // 選択領域の表示を更新
            UpdateSelectedRegion();
        }

        public async Task Select2DRegion(double relativeX, double relativeY)
        {
            if (CurrentSelectionMode.Value != SelectionMode.Fill2DSelection)
                return;

            var renderedImage = _image.RenderImage();
            var bitmapImage = renderedImage.As<WriteableBitmap>();

            Point3D seedPoint = new Point3D(relativeX * bitmapImage.PixelWidth,
                relativeY * bitmapImage.PixelHeight,
                ScrollValue.Value);

            IProgressWindow progressWindow = _progressWindowFactory.Create();
            progressWindow.SetWindowTitle("2D領域選択中");
            progressWindow.Start();
            progressWindow.SetStatusText("2次元塗りつぶし選択を実行中...");

            var progress = new Progress<(int value, string text)>(data =>
            {
                progressWindow.SetStatusText(data.text);
                progressWindow.SetProgress(data.value);
            });

            int threshold = 220; // しきい値は適切な値に変更してください
            await Task.Run(() =>
                _regionSelector.Select2DRegion(seedPoint, threshold));

            progressWindow.End();

            // 選択領域の表示を更新
            UpdateSelectedRegion();
        }

        private void UpdateSelectedRegion()
        {
            if (_image == null || _regionSelector == null)
                return;

            var renderedImage = _image.RenderImage();
            var bitmapImage = renderedImage.As<WriteableBitmap>();

            // 新しいWriteableBitmapを作成し、透明な背景で初期化
            var overlayBitmap = new WriteableBitmap(bitmapImage.PixelWidth,
                bitmapImage.PixelHeight, bitmapImage.DpiX, bitmapImage.DpiY,
                PixelFormats.Bgra32, null);
            var stride = overlayBitmap.PixelWidth * 4;
            var pixels = new byte[overlayBitmap.PixelHeight * stride];

            // 選択された領域を取得
            var selectedRegion = _regionSelector.GetSelectedRegion();

            // 選択された領域を描画
            foreach (var point in selectedRegion.SelectedVoxels)
            {
                if (point.Z == ScrollValue.Value) // 現在のスライスのみ描画
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
            OverlayImageSource.Value = scaledBitmap;
        }

        public void EditRegion(Point3D point, bool isAdd)
        {
            _regionSelector.EditRegion(point, isAdd);
            // 選択領域の表示を更新
        }
    }
}
