using System.IO;
using System.Windows;
using System.Windows.Media.Media3D;
using DicomApp.BloodVesselExtraction.Models;
using DicomApp.BloodVesselExtraction.PresenterInterface;

namespace DicomApp.BloodVesselExtraction.UseCases
{
    public class ManageBloodVesselRegionUseCase
    {
        private readonly BloodVessel3DRegionSelector _regionSelector;

        private readonly IManageBloodVesselRegionPresenter
            _manageBloodVesselRegionPresenter;

        public ManageBloodVesselRegionUseCase(
            BloodVessel3DRegionSelector regionSelector,
            IManageBloodVesselRegionPresenter manageBloodVesselRegionPresenter)
        {
            _regionSelector = regionSelector;
            _manageBloodVesselRegionPresenter =
                manageBloodVesselRegionPresenter;
        }

        public void EndRegionSelection()
        {
            _regionSelector.EndSelection();
            var selectedRegion = _regionSelector.GetSelectedRegion();
            _manageBloodVesselRegionPresenter.InitializeRegionSelection(
                selectedRegion);
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
                    var threshold = _regionSelector.Threshold;
                    var thresholdUpperLimit =
                        _regionSelector.ThresholdUpperLimit;

                    // 選択された領域としきい値をファイルに保存する
                    SaveRegionToFile(selectedFile, selectedRegion, threshold,
                        thresholdUpperLimit);

                    MessageBox.Show($"選択された領域としきい値を {selectedFile} に保存しました。",
                        "保存完了",
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

                    // ファイルから領域としきい値を読み込む
                    var (loadedRegion, loadedThreshold,
                            loadedThresholdUpperLimit) =
                        LoadRegionFromFile(selectedFile);
                    // 読み込んだ領域としきい値を_regionSelectorに設定
                    _regionSelector.SetSelectedRegion(loadedRegion,
                        loadedThreshold, loadedThresholdUpperLimit);
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
            BloodVessel3DRegion region, int threshold, int thresholdUpperLimit)
        {
            // 選択された領域としきい値をファイルに保存する処理を実装
            using var stream = new FileStream(filePath, FileMode.Create);
            using var writer = new BinaryWriter(stream);
            writer.Write(threshold);
            writer.Write(thresholdUpperLimit);
            writer.Write(region.SelectedVoxels.Count);
            foreach (var voxel in region.SelectedVoxels)
            {
                writer.Write(voxel.X);
                writer.Write(voxel.Y);
                writer.Write(voxel.Z);
            }
        }

        private (BloodVessel3DRegion, int, int) LoadRegionFromFile(
            string filePath)
        {
            var region = new BloodVessel3DRegion();

            using var stream = new FileStream(filePath, FileMode.Open);
            using var reader = new BinaryReader(stream);
            int threshold = reader.ReadInt32();
            int thresholdUpperLimit = reader.ReadInt32();
            int voxelCount = reader.ReadInt32();
            for (int i = 0; i < voxelCount; i++)
            {
                int x = reader.ReadInt32();
                int y = reader.ReadInt32();
                int z = reader.ReadInt32();
                region.AddVoxel(new Point3D(x, y, z));
            }

            return (region, threshold, thresholdUpperLimit);
        }

        private void UpdateSelectedRegion()
        {
            var selectedRegion = _regionSelector.GetSelectedRegion();
            _manageBloodVesselRegionPresenter.UpdateSelectedRegion(
                selectedRegion);
        }
    }
}
