using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Media3D;
using DicomApp.MainUseCases.PresenterInterface;

namespace DicomApp.MainUseCases.UseCases
{
    public class SaveModel3dUseCase
    {
        private readonly IProgressWindowFactory _progressWindowFactory;

        public SaveModel3dUseCase(IProgressWindowFactory progressWindowFactory)
        {
            _progressWindowFactory = progressWindowFactory;
        }

        public async Task ExecuteAsync(Model3DGroup model)
        {
            if (model == null)
            {
                throw new InvalidOperationException("現在のモデルが存在しません。");
            }

            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "3Dモデルファイル (*.obj)|*.obj|すべてのファイル (*.*)|*.*",
                DefaultExt = ".obj"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                var progressWindow = _progressWindowFactory.Create();
                string filePath = saveFileDialog.FileName;
                try
                {
                    progressWindow.SetWindowTitle("モデル保存中");
                    progressWindow.Start();
                    progressWindow.SetStatusText("モデルを保存しています...");

                    await Task.Run(() =>
                        SaveModelToFile(model, filePath, progressWindow));

                    progressWindow.End();
                    MessageBox.Show($"モデルを {filePath} に保存しました。", "保存完了",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    progressWindow.End();
                    MessageBox.Show($"モデルの保存中にエラーが発生しました: {ex.Message}", "エラー",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async Task SaveModelToFile(Model3DGroup model, string filePath,
            IProgressWindow progressWindow)
        {
            using var writer = new StreamWriter(filePath);
            int totalVertices = model.Children.OfType<GeometryModel3D>()
                .Sum(m => ((MeshGeometry3D)m.Geometry).Positions.Count);
            int processedVertices = 0;

            foreach (var model3D in model.Children)
            {
                if (model3D is GeometryModel3D {Geometry: MeshGeometry3D mesh})
                {
                    // 頂点を書き込む
                    foreach (Point3D point in mesh.Positions)
                    {
                        writer.WriteLine($"v {point.X} {point.Y} {point.Z}");

                        processedVertices++;
                        if (processedVertices % 1000 == 0 ||
                            processedVertices == totalVertices)
                        {
                            double progress = (double)processedVertices /
                                totalVertices * 100;
                            progressWindow.SetProgress(progress);
                        }
                    }

                    // 面を書き込む
                    for (int i = 0; i < mesh.TriangleIndices.Count; i += 3)
                    {
                        writer.WriteLine(
                            $"f {mesh.TriangleIndices[i] + 1} {mesh.TriangleIndices[i + 1] + 1} {mesh.TriangleIndices[i + 2] + 1}");
                    }
                }
            }
        }
    }
}
