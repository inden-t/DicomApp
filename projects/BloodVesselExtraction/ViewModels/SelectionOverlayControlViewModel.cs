﻿using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using DicomApp.BloodVesselExtraction.Models;
using DicomApp.BloodVesselExtraction.UseCases;
using DicomApp.WpfUtilities.ViewModels;
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
        private BloodVessel3DRegion _selectedRegion = new();

        private readonly Dictionary<int, WriteableBitmap> _overlayBitmaps =
            new();

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
        public ReactiveProperty<int> SliceIndex { get; set; } = new();
        public double Zoom { get; set; } = 1.0;

        public Func<int, WriteableBitmap?> GetSliceImage { get; set; } =
            _ => null;

        public WriteableBitmap? CurrentSliceImage =>
            GetSliceImage(SliceIndex.Value);

        public void InitializeDependencies(
            Select3DBloodVesselRegionUseCase select3DBloodVesselRegionUseCase)
        {
            _select3DBloodVesselRegionUseCase =
                select3DBloodVesselRegionUseCase;
        }

        public void OnClick(double relativeX, double relativeY)
        {
            var currentSliceImage = CurrentSliceImage;
            if (currentSliceImage == null) return;

            Point3D seedPoint = new Point3D(
                relativeX * currentSliceImage.PixelWidth,
                relativeY * currentSliceImage.PixelHeight, SliceIndex.Value);

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


        public void SetSelectedRegion(BloodVessel3DRegion selectedRegion)
        {
            _selectedRegion = selectedRegion;
            _overlayBitmaps.Clear();
            UpdateSelectedRegion();
        }

        public void UpdateSelectedRegion()
        {
            if (_selectedRegion == null)
                return;

            int currentSlice = SliceIndex.Value;
            var overlayBitmap = GetOverlayBitmap(currentSlice);
            if (overlayBitmap == null) return;

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

        private WriteableBitmap? GetOverlayBitmap(int sliceIndex)
        {
            if (!_overlayBitmaps.TryGetValue(sliceIndex, out var overlayBitmap))
            {
                var currentSliceImage = GetSliceImage(sliceIndex);
                if (currentSliceImage == null) return null;

                // 現在のスライスのoverlayBitmapがキャッシュにない場合、新しく作成
                overlayBitmap = new WriteableBitmap(
                    currentSliceImage.PixelWidth, currentSliceImage.PixelHeight,
                    currentSliceImage.DpiX, currentSliceImage.DpiY,
                    PixelFormats.Bgra32, null);
                var stride = overlayBitmap.PixelWidth * 4;
                var pixels = new byte[overlayBitmap.PixelHeight * stride];

                // 選択された領域を描画
                foreach (var point in _selectedRegion.SelectedVoxels)
                {
                    if (point.Z == sliceIndex) // 現在のスライスのみ描画
                    {
                        int x = point.X;
                        int y = point.Y;
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

                // キャッシュに保存
                _overlayBitmaps[sliceIndex] = overlayBitmap;
            }

            return overlayBitmap;
        }
    }
}
