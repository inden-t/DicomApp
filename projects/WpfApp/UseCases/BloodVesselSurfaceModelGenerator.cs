using System;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using DicomApp.Models;

namespace DicomApp.UseCases
{
    public class BloodVesselSurfaceModelGenerator
    {
        private const byte _intensityIso = 200;
        private const byte _intensityMax = 255;
        private const byte _intensityMin = 0;

        public async Task<Model3DGroup> GenerateModelAsync(
            BloodVessel3DRegion region, int threshold, IProgress<(int value, string text)> progress)
        {
            return await Task.Run(() => CreateSurfaceModel(region, threshold, progress));
        }

        private Model3DGroup CreateSurfaceModel(BloodVessel3DRegion region, int threshold, IProgress<(int value, string text)> progress)
        {
            var model3DGroup = new Model3DGroup();

            // 3次元ボクセルグリッドを作成
            var boundingBox = GetBoundingBox(region);
            int width = boundingBox.Width;
            int height = boundingBox.Height;
            int depth = boundingBox.Depth;
            var voxelGrid = new double[width, height, depth];

            // ボクセルグリッドを初期化
            InitializeVoxelGrid(voxelGrid, region, boundingBox);

            // Marching Cubesアルゴリズムを使用してサーフェスモデルを生成
            var surfaceGeometry = CreateSurfaceFromVoxels(voxelGrid, threshold, progress);

            // マテリアルを作成
            var materialGroup = CreateMaterial();

            var surfaceModel = new GeometryModel3D(surfaceGeometry, materialGroup);
            surfaceModel.BackMaterial = materialGroup; // 裏面にも同じマテリアルを適用

            // 光源を追加
            var directionalLight = new DirectionalLight(Color.FromRgb(200, 200, 200), new Vector3D(-1, -1, -1));

            model3DGroup.Children.Add(surfaceModel);
            model3DGroup.Children.Add(directionalLight);
            model3DGroup.Freeze();

            return model3DGroup;
        }

        private (int X, int Y, int Z, int Width, int Height, int Depth) GetBoundingBox(BloodVessel3DRegion region)
        {
            int minX = int.MaxValue, minY = int.MaxValue, minZ = int.MaxValue;
            int maxX = int.MinValue, maxY = int.MinValue, maxZ = int.MinValue;

            foreach (var voxel in region.SelectedVoxels)
            {
                minX = Math.Min(minX, voxel.X);
                minY = Math.Min(minY, voxel.Y);
                minZ = Math.Min(minZ, voxel.Z);
                maxX = Math.Max(maxX, voxel.X);
                maxY = Math.Max(maxY, voxel.Y);
                maxZ = Math.Max(maxZ, voxel.Z);
            }

            return (minX, minY, minZ, maxX - minX + 1, maxY - minY + 1, maxZ - minZ + 1);
        }

        private void InitializeVoxelGrid(double[,,] voxelGrid, BloodVessel3DRegion region, (int X, int Y, int Z, int Width, int Height, int Depth) boundingBox)
        {
            foreach (var voxel in region.SelectedVoxels)
            {
                int x = voxel.X - boundingBox.X;
                int y = voxel.Y - boundingBox.Y;
                int z = voxel.Z - boundingBox.Z;
                voxelGrid[x, y, z] = 1.0; // 選択された領域を1.0（最大値）に設定
            }
        }

        private MeshGeometry3D CreateSurfaceFromVoxels(double[,,] voxelGrid, int threshold, IProgress<(int value, string text)> progress)
        {
            var mesh = new MeshGeometry3D();
            int width = voxelGrid.GetLength(0);
            int height = voxelGrid.GetLength(1);
            int depth = voxelGrid.GetLength(2);

            int totalVoxels = (width - 1) * (height - 1) * (depth - 1);
            int processedVoxels = 0;

            double isoValue = threshold / 255.0; // しきい値を0.0～1.0の範囲に正規化

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
                        if (voxelGrid[x + 1, y + 1, z] > isoValue) cubeIndex |= 4;
                        if (voxelGrid[x, y + 1, z] > isoValue) cubeIndex |= 8;
                        if (voxelGrid[x, y, z + 1] > isoValue) cubeIndex |= 16;
                        if (voxelGrid[x + 1, y, z + 1] > isoValue) cubeIndex |= 32;
                        if (voxelGrid[x + 1, y + 1, z + 1] > isoValue) cubeIndex |= 64;
                        if (voxelGrid[x, y + 1, z + 1] > isoValue) cubeIndex |= 128;

                        // ルックアップテーブルを使用して三角形を生成
                        var triangles = MarchingCubesLookupTable.GetTriangles(cubeIndex);
                        foreach (var triangle in triangles)
                        {
                            foreach (var edge in triangle)
                            {
                                var v = GetInterpolatedVertexPosition(edge, x, y, z, voxelGrid, isoValue);
                                mesh.Positions.Add(v);
                                mesh.TriangleIndices.Add(mesh.Positions.Count - 1);
                            }
                        }

                        processedVoxels++;
                        if (processedVoxels % 1000 == 0 || processedVoxels == totalVoxels)
                        {
                            double progressValue = (double)processedVoxels / totalVoxels * 100;
                            progress.Report(((int)progressValue, $"サーフェスモデル(線形補間)を生成中...\n" +
                                $"処理済みボクセル数: {processedVoxels}/{totalVoxels}\n" +
                                $"生成されたポイント数: {mesh.Positions.Count}\n" +
                                $"生成された三角形の数: {mesh.TriangleIndices.Count / 3}"));
                        }
                    }
                }
            }

            return mesh;
        }

        private Point3D GetInterpolatedVertexPosition(int edge, int x, int y, int z, double[,,] voxelGrid, double isoValue)
        {
            int x1, y1, z1, x2, y2, z2;
            MarchingCubesLookupTable.GetEdgeEndpoints(edge, x, y, z, out x1, out y1, out z1, out x2, out y2, out z2);
            double mu = GetInterpolationFactor(voxelGrid[x1, y1, z1], voxelGrid[x2, y2, z2], isoValue);
            return new Point3D(x1 + mu * (x2 - x1), y1 + mu * (y2 - y1), z1 + mu * (z2 - z1));
        }

        private double GetInterpolationFactor(double v1, double v2, double isoValue)
        {
            if (Math.Abs(v1 - v2) < 0.00001)
                return 0.5;

            return (isoValue - v1) / (v2 - v1);
        }

        private MaterialGroup CreateMaterial()
        {
            var materialGroup = new MaterialGroup();

            // 拡散反射（基本的な色と陰影）
            materialGroup.Children.Add(new DiffuseMaterial(new SolidColorBrush(Color.FromRgb(200, 0, 0))));

            // 鏡面反射（ハイライト）
            materialGroup.Children.Add(new SpecularMaterial(new SolidColorBrush(Color.FromArgb(100, 255, 100, 100)), 10));

            return materialGroup;
        }
    }
}
