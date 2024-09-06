using System;
using System.Windows;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using DicomApp.Models;

namespace DicomApp.UseCases
{
    public class DisplaySurfaceModelLinearInterpolationUseCase
    {
        private const byte _intensityIso = 200;
        private const byte _intensityMax = 255;
        private const byte _intensityMin = 0;

        private readonly FileManager _fileManager;
        private readonly IModel3dViewerFactory _viewerFactory;
        private readonly IProgressWindowFactory _progressWindowFactory;

        private IModel3dViewer _viewer;
        private IProgressWindow _progressWindow;

        public DisplaySurfaceModelLinearInterpolationUseCase(
            FileManager fileManager,
            IModel3dViewerFactory viewerFactory,
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
                _progressWindow.SetWindowTitle("モデル生成中");
                _progressWindow.Start();
                _progressWindow.SetStatusText("サーフェスモデル(線形補間)を生成中...");

                var model3DGroup =
                    await Task.Run(() => CreateSurfaceModel());

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

        private Model3DGroup CreateSurfaceModel()
        {
            var model3DGroup = new Model3DGroup();
            int totalFiles = _fileManager.DicomFiles.Count;

            // 3次元ボクセルグリッドを作成
            int width = _fileManager.DicomFiles[0].GetImage().Width;
            int height = _fileManager.DicomFiles[0].GetImage().Height;
            var voxelGrid = new double[width, height, totalFiles];

            for (int z = 0; z < totalFiles; z++)
            {
                var dicomFile = _fileManager.DicomFiles[z];
                var image = dicomFile.GetImage();
                var renderedImage = image.RenderImage()
                    .As<System.Windows.Media.Imaging.WriteableBitmap>();
                var stride = width * 4; // 4 bytes per pixel (BGRA)
                var pixels = new byte[height * stride];
                renderedImage.CopyPixels(pixels, stride, 0);

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int index = (y * stride) + (x * 4);
                        byte intensity = pixels[index]; // Blue channel
                        intensity = intensity > _intensityMax
                            ? _intensityMax
                            : intensity;
                        intensity = intensity < _intensityMin
                            ? _intensityMin
                            : intensity;
                        voxelGrid[x, y, z] = (intensity - _intensityMin) /
                                             (double)(_intensityMax -
                                                 _intensityMin);
                        // 0.0～1.0の範囲に正規化
                    }
                }

                double progress = (z + 1) / (double)totalFiles * 100;
                _progressWindow.SetProgress(progress);
                _progressWindow.SetStatusText(
                    $"サーフェスモデル(線形補間)を生成中...\n{z + 1}/{totalFiles} files");
            }

            // Marching Cubesアルゴリズムを使用してサーフェスモデルを生成
            var surfaceGeometry = CreateSurfaceFromVoxels(voxelGrid);

            // 陰影のあるマテリアルを作成
            var materialGroup = new MaterialGroup();

            // 拡散反射（基本的な色と陰影）
            materialGroup.Children.Add(
                new DiffuseMaterial(
                    new SolidColorBrush(Color.FromRgb(200, 200, 200))));

            // 鏡面反射（ハイライト）- 反射を弱く、広く
            materialGroup.Children.Add(new SpecularMaterial(
                new SolidColorBrush(Color.FromArgb(100, 100, 100, 255)), 10));

            var surfaceModel =
                new GeometryModel3D(surfaceGeometry, materialGroup);
            surfaceModel.BackMaterial = materialGroup; // 裏面にも同じマテリアルを適用

            // 光源を追加
            var directionalLight =
                new DirectionalLight(Color.FromRgb(200, 200, 200),
                    new Vector3D(-1, -1, -1));

            model3DGroup.Children.Add(surfaceModel);
            model3DGroup.Children.Add(directionalLight);
            model3DGroup.Freeze();

            return model3DGroup;
        }

        private MeshGeometry3D CreateSurfaceFromVoxels(double[,,] voxelGrid)
        {
            var mesh = new MeshGeometry3D();
            int width = voxelGrid.GetLength(0);
            int height = voxelGrid.GetLength(1);
            int depth = voxelGrid.GetLength(2);

            int totalVoxels = (width - 1) * (height - 1) * (depth - 1);
            int processedVoxels = 0;

            double isoValue = (_intensityIso - _intensityMin) /
                              (double)(_intensityMax - _intensityMin); // 等値面の閾値

            for (int x = 0; x < width - 1; x++)
            {
                for (int y = 0; y < height - 1; y++)
                {
                    for (int z = 0; z < depth - 1; z++)
                    {
                        // Marching Cubesアルゴリズムの実装
                        int cubeIndex = 0;
                        if (voxelGrid[x, y, z] > isoValue) cubeIndex |= 1;
                        if (voxelGrid[x + 1, y, z] > isoValue) cubeIndex |= 2;
                        if (voxelGrid[x + 1, y + 1, z] > isoValue)
                            cubeIndex |= 4;
                        if (voxelGrid[x, y + 1, z] > isoValue) cubeIndex |= 8;
                        if (voxelGrid[x, y, z + 1] > isoValue) cubeIndex |= 16;
                        if (voxelGrid[x + 1, y, z + 1] > isoValue)
                            cubeIndex |= 32;
                        if (voxelGrid[x + 1, y + 1, z + 1] > isoValue)
                            cubeIndex |= 64;
                        if (voxelGrid[x, y + 1, z + 1] > isoValue)
                            cubeIndex |= 128;

                        // ルックアップテーブルを使用して三角形を生成
                        var triangles =
                            MarchingCubesLookupTable.GetTriangles(cubeIndex);
                        foreach (var triangle in triangles)
                        {
                            foreach (var edge in triangle)
                            {
                                var v = GetInterpolatedVertexPosition(edge, x,
                                    y, z, voxelGrid, isoValue);
                                // X座標を反転
                                v.X = width - 1 - v.X;
                                mesh.Positions.Add(v);
                                mesh.TriangleIndices.Add(mesh.Positions.Count -
                                    1);
                            }
                        }

                        processedVoxels++;
                        if (processedVoxels % 1000 == 0 ||
                            processedVoxels == totalVoxels)
                        {
                            double progress = (double)processedVoxels /
                                totalVoxels * 100;
                            _progressWindow.SetProgress(progress);
                            _progressWindow.SetStatusText(
                                $"サーフェスモデル(線形補間)を生成中...\n" +
                                $"処理済みボクセル数: {processedVoxels}/{totalVoxels}\n" +
                                $"生成されたポイント数: {mesh.Positions.Count}\n" +
                                $"生成された三角形の数: {mesh.TriangleIndices.Count / 3}");
                        }
                    }
                }
            }

            return mesh;
        }

        private Point3D GetInterpolatedVertexPosition(int edge, int x, int y,
            int z, double[,,] voxelGrid, double isoValue)
        {
            int x1, y1, z1, x2, y2, z2;
            MarchingCubesLookupTable.GetEdgeEndpoints(edge, x, y, z, out x1,
                out y1, out z1, out x2, out y2, out z2);
            double mu = GetInterpolationFactor(voxelGrid[x1, y1, z1],
                voxelGrid[x2, y2, z2], isoValue);
            var v = new Point3D(x1 + mu * (x2 - x1), y1 + mu * (y2 - y1),
                z1 + mu * (z2 - z1));

            return v;
        }

        private double GetInterpolationFactor(double v1, double v2,
            double isoValue)
        {
            if (Math.Abs(v1 - v2) < 0.00001)
                return 0.5;

            double mu = (isoValue - v1) / (v2 - v1);

            return mu;
        }
    }
}
