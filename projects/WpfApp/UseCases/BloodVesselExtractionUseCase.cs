using System.Windows;
using System.Windows.Media.Media3D;
using DicomApp.Models;
using DicomApp.UseCases;

public class BloodVesselExtractionUseCase
{
    private readonly FileManager _fileManager;
    private readonly BloodVessel3DRegionSelector _regionSelector;
    private readonly BloodVesselSurfaceModelGenerator _modelGenerator;
    private readonly IModel3dViewerFactory _viewerFactory;
    private readonly IProgressWindowFactory _progressWindowFactory;

    public BloodVesselExtractionUseCase(
        FileManager fileManager, BloodVessel3DRegionSelector regionSelector,
        BloodVesselSurfaceModelGenerator modelGenerator,
        IModel3dViewerFactory viewerFactory,
        IProgressWindowFactory progressWindowFactory)
    {
        _fileManager = fileManager;
        _regionSelector = regionSelector;
        _modelGenerator = modelGenerator;
        _viewerFactory = viewerFactory;
        _progressWindowFactory = progressWindowFactory;
    }

    public async Task ExtractBloodVesselAsync()
    {
        IProgressWindow progressWindow = _progressWindowFactory.Create();
        progressWindow.SetWindowTitle("モデル生成中");
        progressWindow.Start();
        progressWindow.SetStatusText("サーフェスモデルを生成中...");

        try
        {
            var progress = new Progress<(int value, string text)>(data =>
            {
                progressWindow.SetStatusText(data.text);
                progressWindow.SetProgress(data.value);
            });

            int threshold = 220;
            var region = _regionSelector.GetSelectedRegion();
            var model3DGroup =
                await _modelGenerator.GenerateModelAsync(_fileManager, region,
                    threshold,
                    progress);

            var viewer = _viewerFactory.Create();
            viewer.SetModel(model3DGroup);

            progressWindow.End();
            viewer.Show();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"サーフェスモデルの生成中にエラーが発生しました: {ex.Message}", "エラー",
                MessageBoxButton.OK, MessageBoxImage.Error);
            Console.WriteLine($"詳細なエラー情報: {ex}");
        }
        finally
        {
            progressWindow.End();
            System.Windows.Input.Mouse.OverrideCursor = null;
        }
    }

    //public async Task SaveModel()
    //{
    //    IProgressWindow progressWindow = _progressWindowFactory.Create();
    //    progressWindow.SetWindowTitle("モデル保存中");
    //    progressWindow.Start();
    //    progressWindow.SetStatusText("サーフェスモデルを保存中...");

    //    try
    //    {
    //        // ファイル保存ダイアログを表示
    //        var saveFileDialog = new Microsoft.Win32.SaveFileDialog
    //        {
    //            Filter =
    //                "3D Model Files (*.stl, *.obj, *.ply)|*.stl;*.obj;*.ply",
    //            DefaultExt = ".stl"
    //        };

    //        if (saveFileDialog.ShowDialog() == true)
    //        {
    //            var selectedFile = saveFileDialog.FileName;
    //            var region = _regionSelector.GetSelectedRegion();
    //            var model3DGroup = await _modelGenerator.GenerateModelAsync(
    //                _fileManager, region, 220, new Progress<(int, string)>());

    //            // 選択されたファイル形式に応じて保存処理を行う
    //            if (selectedFile.EndsWith(".stl"))
    //            {
    //                await SaveSTLModel(selectedFile, model3DGroup);
    //            }
    //            else if (selectedFile.EndsWith(".obj"))
    //            {
    //                await SaveOBJModel(selectedFile, model3DGroup);
    //            }
    //            else if (selectedFile.EndsWith(".ply"))
    //            {
    //                await SavePLYModel(selectedFile, model3DGroup);
    //            }

    //            MessageBox.Show($"モデルを {selectedFile} に保存しました。", "保存完了",
    //                MessageBoxButton.OK, MessageBoxImage.Information);
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        MessageBox.Show($"モデルの保存中にエラーが発生しました: {ex.Message}", "エラー",
    //            MessageBoxButton.OK, MessageBoxImage.Error);
    //        Console.WriteLine($"詳細なエラー情報: {ex}");
    //    }
    //    finally
    //    {
    //        progressWindow.End();
    //    }
    //}

    //private async Task SaveSTLModel(string filePath, Model3DGroup model3DGroup)
    //{
    //    // STLファイルの保存処理を実装
    //    // 例: https://github.com/ericnewton76/gmf
    //}

    //private async Task SaveOBJModel(string filePath, Model3DGroup model3DGroup)
    //{
    //    // OBJファイルの保存処理を実装
    //}

    //private async Task SavePLYModel(string filePath, Model3DGroup model3DGroup)
    //{
    //    // PLYファイルの保存処理を実装
    //}
}
