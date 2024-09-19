using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using DicomApp.Models;

namespace DicomApp.UseCases
{
    public class ManageBloodVesselRegionUseCase
    {
        private readonly BloodVessel3DRegionSelector _regionSelector;
        private readonly IProgressWindowFactory _progressWindowFactory;

        public ManageBloodVesselRegionUseCase(
            BloodVessel3DRegionSelector regionSelector,
            IProgressWindowFactory progressWindowFactory)
        {
            _regionSelector = regionSelector;
            _progressWindowFactory = progressWindowFactory;
        }

        public async Task SaveSelectedRegionAsync()
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
                    await SaveRegionToFile(selectedFile, selectedRegion);

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

        public async Task LoadSelectedRegionAsync()
        {
            // 領域を読み込むメソッドを実装する場合はここに追加
        }

        private async Task SaveRegionToFile(string filePath,
            BloodVessel3DRegion region)
        {
            // 選択された領域をファイルに保存する処理を実装
            using (var stream = new FileStream(filePath, FileMode.Create))
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write(region.SelectedVoxels.Count);
                foreach (var voxel in region.SelectedVoxels)
                {
                    writer.Write(voxel.X);
                    writer.Write(voxel.Y);
                    writer.Write(voxel.Z);
                }
            }
        }
    }
}
