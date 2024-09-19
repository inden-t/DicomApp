using System.Windows;
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

    public async Task SaveModel()
    {
    }
}
