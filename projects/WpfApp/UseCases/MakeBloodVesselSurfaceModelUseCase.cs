using System;
using System.Windows;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using DicomApp.Models;

namespace DicomApp.UseCases
{
    public class MakeBloodVesselSurfaceModelUseCase
    {
        private readonly FileManager _fileManager;
        private readonly IBloodVesselPointCloud3DViewerFactory _viewerFactory;
        private readonly IProgressWindowFactory _progressWindowFactory;

        private IBloodVesselPointCloud3DViewer _viewer;
        private IProgressWindow _progressWindow;

        public MakeBloodVesselSurfaceModelUseCase(FileManager fileManager,
            IBloodVesselPointCloud3DViewerFactory viewerFactory,
            IProgressWindowFactory progressWindowFactory)
        {
            _fileManager = fileManager;
            _viewerFactory = viewerFactory;
            _progressWindowFactory = progressWindowFactory;
        }

        public async Task ExecuteAsync()
        {
            try
            {
                System.Windows.Input.Mouse.OverrideCursor =
                    System.Windows.Input.Cursors.Wait;

                _progressWindow = _progressWindowFactory.Create();
                _progressWindow.Start();
                _progressWindow.SetStatusText("血管のサーフェスモデルを生成中...");

                var model3DGroup =
                    await Task.Run(() => CreateBloodVesselSurfaceModel());

                _viewer = _viewerFactory.Create();
                _viewer.SetModel(model3DGroup);

                _progressWindow.End();
                _viewer.Show();

                System.Windows.Input.Mouse.OverrideCursor = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"サーフェスモデルの生成中にエラーが発生しました: {ex.Message}", "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine($"詳細なエラー情報: {ex}");
            }
            finally
            {
                _progressWindow.End();
                System.Windows.Input.Mouse.OverrideCursor = null;
            }
        }

        private Model3DGroup CreateBloodVesselSurfaceModel()
        {
            var model3DGroup = new Model3DGroup();
            int totalFiles = _fileManager.DicomFiles.Count;

            var points = new List<Point3D>();

            for (int i = 0; i < totalFiles; i++)
            {
                var dicomFile = _fileManager.DicomFiles[i];
                var image = dicomFile.GetImage();
                var renderedImage = image.RenderImage()
                    .As<System.Windows.Media.Imaging.WriteableBitmap>();
                var width = renderedImage.PixelWidth;
                var height = renderedImage.PixelHeight;
                var stride = width * 4; // 4 bytes per pixel (BGRA)
                var pixels = new byte[height * stride];
                renderedImage.CopyPixels(pixels, stride, 0);

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int index = (y * stride) + (x * 4);
                        byte intensity = pixels[index]; // Blue channel

                        if (intensity > 200) // 血管と思われる明るい部分のしきい値
                        {
                            points.Add(new Point3D(width - 1 - x, y, i));
                        }
                    }
                }

                double progress = (i + 1) / (double)totalFiles * 100;
                _progressWindow.SetProgress(progress);
                _progressWindow.SetStatusText(
                    $"血管のサーフェスモデルを生成中... ({i + 1}/{totalFiles} files)");
            }

            // ポイントクラウドからサーフェスモデルを生成
            var surfaceGeometry = CreateSurfaceFromPoints(points);
            var material = new DiffuseMaterial(new SolidColorBrush(Colors.Red));
            var surfaceModel = new GeometryModel3D(surfaceGeometry, material);

            model3DGroup.Children.Add(surfaceModel);
            model3DGroup.Freeze();

            return model3DGroup;
        }

        private MeshGeometry3D CreateSurfaceFromPoints(List<Point3D> points)
        {
            var mesh = new MeshGeometry3D();
            int gridSize = 10; // グリッドのサイズを調整して、パフォーマンスと品質のバランスを取る

            // 点群をグリッドに分割
            var grid = new Dictionary<(int, int, int), List<Point3D>>();
            foreach (var point in points)
            {
                var key = ((int)(point.X / gridSize), (int)(point.Y / gridSize),
                    (int)(point.Z / gridSize));
                if (!grid.ContainsKey(key))
                    grid[key] = new List<Point3D>();
                grid[key].Add(point);
            }

            // 各グリッドセルの中心点を計算
            var cellCenters = new List<Point3D>();
            foreach (var cell in grid.Values)
            {
                var center = new Point3D(
                    cell.Average(p => p.X),
                    cell.Average(p => p.Y),
                    cell.Average(p => p.Z)
                );
                cellCenters.Add(center);
                mesh.Positions.Add(center);
            }

            // 隣接するセル間に三角形を作成
            for (int i = 0; i < cellCenters.Count - 1; i++)
            {
                for (int j = i + 1; j < cellCenters.Count; j++)
                {
                    var distance = (cellCenters[i] - cellCenters[j]).Length;
                    if (distance < gridSize * 1.5) // 近接しているセル同士を接続
                    {
                        mesh.TriangleIndices.Add(i);
                        mesh.TriangleIndices.Add(j);
                        mesh.TriangleIndices.Add((i + j) / 2); // 簡易的な第三の点

                        _progressWindow.SetStatusText(
                            $"血管のサーフェスモデルを生成中...\n生成されたポイント数: {mesh.Positions.Count}\n生成された三角形の数: {mesh.TriangleIndices.Count / 3}");
                    }
                }
            }

            return mesh;
        }
    }
}
