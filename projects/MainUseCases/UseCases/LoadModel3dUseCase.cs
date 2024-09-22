using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using DicomApp.MainUseCases.PresenterInterface;

namespace DicomApp.MainUseCases.UseCases
{
    public class LoadModel3dUseCase
    {
        private readonly IModel3dViewerFactory _viewerFactory;
        private readonly IProgressWindowFactory _progressWindowFactory;

        public LoadModel3dUseCase(IModel3dViewerFactory viewerFactory,
            IProgressWindowFactory progressWindowFactory)
        {
            _viewerFactory = viewerFactory;
            _progressWindowFactory = progressWindowFactory;
        }

        public async Task ExecuteAsync()
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "3Dモデルファイル (*.obj)|*.obj|すべてのファイル (*.*)|*.*",
                DefaultExt = ".obj"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                IProgressWindow progressWindow =
                    _progressWindowFactory.Create();

                try
                {
                    Mouse.OverrideCursor = Cursors.Wait;

                    progressWindow.SetWindowTitle("モデル読み込み中");
                    progressWindow.Start();
                    progressWindow.SetStatusText("3Dモデルを読み込んでいます...");

                    Model3DGroup loadedModel = await Task.Run(() =>
                        LoadModelFromFile(filePath, progressWindow));

                    var viewer = _viewerFactory.Create();
                    viewer.SetModel(loadedModel);

                    progressWindow.End();
                    viewer.Show();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"モデルの読み込み中にエラーが発生しました: {ex.Message}",
                        "エラー",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    Console.WriteLine($"詳細なエラー情報: {ex}");
                }
                finally
                {
                    progressWindow.End();
                    Mouse.OverrideCursor = null;
                }
            }
        }

        private Model3DGroup LoadModelFromFile(string filePath,
            IProgressWindow progressWindow)
        {
            var model = new Model3DGroup();
            using var reader = new StreamReader(filePath);
            var mesh = new MeshGeometry3D();

            string line;
            long totalLines = File.ReadLines(filePath).Count();
            long processedLines = 0;

            while ((line = reader.ReadLine()) != null)
            {
                var parts = line.Split(' ');
                if (parts[0] == "v")
                {
                    mesh.Positions.Add(new Point3D(
                        double.Parse(parts[1]),
                        double.Parse(parts[2]),
                        double.Parse(parts[3])));
                }
                else if (parts[0] == "f")
                {
                    mesh.TriangleIndices.Add(int.Parse(parts[1]) - 1);
                    mesh.TriangleIndices.Add(int.Parse(parts[2]) - 1);
                    mesh.TriangleIndices.Add(int.Parse(parts[3]) - 1);
                }

                processedLines++;
                if (processedLines % 1000 == 0 || processedLines == totalLines)
                {
                    double progress = (double)processedLines / totalLines * 100;
                    progressWindow.SetProgress(progress);
                    progressWindow.SetStatusText(
                        $"3Dモデルを読み込んでいます... {processedLines}/{totalLines} 行");
                }
            }

            var materialGroup = new MaterialGroup();

            // 拡散反射（基本的な色と陰影）
            materialGroup.Children.Add(
                new DiffuseMaterial(
                    new SolidColorBrush(Color.FromRgb(200, 0, 0))));

            // 鏡面反射（ハイライト）
            materialGroup.Children.Add(new SpecularMaterial(
                new SolidColorBrush(Color.FromArgb(100, 255, 100, 100)), 10));

            var geometryModel = new GeometryModel3D(mesh, materialGroup);

            // 両面レンダリングを有効にする
            geometryModel.BackMaterial = materialGroup;

            // 光源を追加
            var directionalLight =
                new DirectionalLight(Color.FromRgb(200, 200, 200),
                    new Vector3D(-1, -1, -1));

            model.Children.Add(geometryModel);
            model.Children.Add(directionalLight);
            model.Freeze();

            return model;
        }
    }
}
