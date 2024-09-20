using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Media3D;
using DicomApp.Models;

namespace DicomApp.UseCases
{
    public class ManageBloodVesselRegionUseCase
    {
        private readonly BloodVessel3DRegionSelector _regionSelector;
        private readonly IImageViewerPresenter _imageViewerPresenter;

        public ManageBloodVesselRegionUseCase(
            BloodVessel3DRegionSelector regionSelector,
            IImageViewerPresenter imageViewerPresenter)
        {
            _regionSelector = regionSelector;
            _imageViewerPresenter = imageViewerPresenter;
        }

        public void InitializeRegionSelector()
        {
            _regionSelector.Initialize();
            _imageViewerPresenter.SetSelectionModeActive(false);
            UpdateSelectedRegion();
        }

        public void UndoSelection()
        {
            if (_regionSelector.CanUndo())
            {
                _regionSelector.Undo();
                UpdateSelectedRegion();
            }
        }

        public void RedoSelection()
        {
            if (_regionSelector.CanRedo())
            {
                _regionSelector.Redo();
                UpdateSelectedRegion();
            }
        }

        public void ClearAllSelection()
        {
            if (_regionSelector != null)
            {
                _regionSelector.ClearAllRegions();
                UpdateSelectedRegion();
            }
        }

        public void SaveSelectedRegion()
        {
            try
            {
                // ファイル保存ダイアログを表示
                var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "Region Data (*.region)|*.region",
                    DefaultExt = ".region"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    var selectedFile = saveFileDialog.FileName;
                    var selectedRegion = _regionSelector.GetSelectedRegion();

                    // 選択された領域をファイルに保存する
                    SaveRegionToFile(selectedFile, selectedRegion);

                    MessageBox.Show($"選択された領域を {selectedFile} に保存しました。", "保存完了",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"領域の保存中にエラーが発生しました: {ex.Message}", "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine($"詳細なエラー情報: {ex}");
            }
        }

        public void LoadSelectedRegion()
        {
            try
            {
                // ファイル選択ダイアログを表示
                var openFileDialog = new Microsoft.Win32.OpenFileDialog
                {
                    Filter = "Region Data (*.region)|*.region",
                    DefaultExt = ".region"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    var selectedFile = openFileDialog.FileName;

                    // ファイルから領域を読み込む
                    var loadedRegion = LoadRegionFromFile(selectedFile);

                    // 読み込んだ領域を_regionSelectorに設定
                    _regionSelector.SetSelectedRegion(loadedRegion);
                }

                UpdateSelectedRegion();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"領域の読み込み中にエラーが発生しました: {ex.Message}", "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine($"詳細なエラー情報: {ex}");
            }
        }

        private void SaveRegionToFile(string filePath,
            BloodVessel3DRegion region)
        {
            // 選択された領域をファイルに保存する処理を実装
            using var stream = new FileStream(filePath, FileMode.Create);
            using var writer = new BinaryWriter(stream);
            writer.Write(region.SelectedVoxels.Count);
            foreach (var voxel in region.SelectedVoxels)
            {
                writer.Write(voxel.X);
                writer.Write(voxel.Y);
                writer.Write(voxel.Z);
            }
        }

        private BloodVessel3DRegion LoadRegionFromFile(
            string filePath)
        {
            var region = new BloodVessel3DRegion();

            using var stream = new FileStream(filePath, FileMode.Open);
            using var reader = new BinaryReader(stream);
            int voxelCount = reader.ReadInt32();
            for (int i = 0; i < voxelCount; i++)
            {
                int x = reader.ReadInt32();
                int y = reader.ReadInt32();
                int z = reader.ReadInt32();
                region.AddVoxel(new Point3D(x, y, z));
            }

            return region;
        }

        private void UpdateSelectedRegion()
        {
            var selectedRegion = _regionSelector.GetSelectedRegion();
            _imageViewerPresenter.SetSelectedRegion(selectedRegion);
        }
    }
}
