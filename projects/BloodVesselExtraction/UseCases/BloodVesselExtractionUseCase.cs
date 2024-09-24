﻿using System.Windows;
using DicomApp.BloodVesselExtraction.Models;
using DicomApp.CoreModels.Models;
using DicomApp.MainUseCases.PresenterInterface;

namespace DicomApp.BloodVesselExtraction.UseCases
{
    public class BloodVesselExtractionUseCase
    {
        private readonly FileManager _fileManager;
        private readonly BloodVessel3DRegionSelector _regionSelector;
        private readonly BloodVesselSurfaceModelGenerator _modelGenerator;
        private readonly IModel3dViewerFactory _viewerFactory;
        private readonly IProgressWindowFactory _progressWindowFactory;

        private int _threshold = 220;
        private int _thresholdUpperLimit = 255;

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

        public void SetThreshold(int threshold, int thresholdUpperLimit)
        {
            _threshold = threshold;
            _thresholdUpperLimit = thresholdUpperLimit;
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

                var region = _regionSelector.GetSelectedRegion();
                var model3DGroup =
                    await _modelGenerator.GenerateModelAsync(_fileManager,
                        region, _threshold, _thresholdUpperLimit, progress);

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
    }
}
