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
            FileManager fileManager, BloodVessel3DRegion region, int threshold,
            IProgress<(int value, string text)> progress)
        {
            return await Task.Run(() =>
                CreateSurfaceModel(fileManager, region, threshold, progress));
        }

        private Model3DGroup CreateSurfaceModel(FileManager fileManager,
            BloodVessel3DRegion region, int threshold,
            IProgress<(int value, string text)> progress)
        {
            var model3DGroup = new Model3DGroup();

            // 3次元ボクセルグリッドを作成
            var boundingBox = GetBoundingBox(region);
            int width = boundingBox.Width;
            int height = boundingBox.Height;
            int depth = boundingBox.Depth;

            // 画像の色情報を取得
            var imageIntensities =
                GetImageIntensities(fileManager, boundingBox);

            // Marching Cubesアルゴリズムを使用してサーフェスモデルを生成
            var surfaceGeometry = CreateSurfaceFromVoxels(region,
                imageIntensities, threshold, progress, width);

            // マテリアルを作成
            var materialGroup = CreateMaterial();

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

        private (int X, int Y, int Z, int Width, int Height, int Depth)
            GetBoundingBox(BloodVessel3DRegion region)
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

            return (minX, minY, minZ, maxX - minX + 1, maxY - minY + 1,
                maxZ - minZ + 1);
        }

        private void InitializeVoxelGrid(double[,,] voxelGrid,
            BloodVessel3DRegion region,
            (int X, int Y, int Z, int Width, int Height, int Depth) boundingBox)
        {
            foreach (var voxel in region.SelectedVoxels)
            {
                int x = voxel.X - boundingBox.X;
                int y = voxel.Y - boundingBox.Y;
                int z = voxel.Z - boundingBox.Z;
                voxelGrid[x, y, z] = 1.0; // 選択された領域を1.0（最大値）に設定
            }
        }

        private MeshGeometry3D CreateSurfaceFromVoxels(
            BloodVessel3DRegion region, double[,,] imageIntensities,
            int threshold, IProgress<(int value, string text)> progress,
            int totalWidth)
        {
            var mesh = new MeshGeometry3D();
            var boundingBox = GetBoundingBox(region);
            int width = boundingBox.Width;
            int height = boundingBox.Height;
            int depth = boundingBox.Depth;

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
                        if (region.ContainsVoxel(new Point3D(x + boundingBox.X,
                                y + boundingBox.Y, z + boundingBox.Z)))
                            cubeIndex |= 1;
                        if (region.ContainsVoxel(new Point3D(
                                x + 1 + boundingBox.X, y + boundingBox.Y,
                                z + boundingBox.Z))) cubeIndex |= 2;
                        if (region.ContainsVoxel(new Point3D(
                                x + 1 + boundingBox.X, y + 1 + boundingBox.Y,
                                z + boundingBox.Z))) cubeIndex |= 4;
                        if (region.ContainsVoxel(new Point3D(x + boundingBox.X,
                                y + 1 + boundingBox.Y, z + boundingBox.Z)))
                            cubeIndex |= 8;
                        if (region.ContainsVoxel(new Point3D(x + boundingBox.X,
                                y + boundingBox.Y, z + 1 + boundingBox.Z)))
                            cubeIndex |= 16;
                        if (region.ContainsVoxel(new Point3D(
                                x + 1 + boundingBox.X, y + boundingBox.Y,
                                z + 1 + boundingBox.Z))) cubeIndex |= 32;
                        if (region.ContainsVoxel(new Point3D(
                                x + 1 + boundingBox.X, y + 1 + boundingBox.Y,
                                z + 1 + boundingBox.Z))) cubeIndex |= 64;
                        if (region.ContainsVoxel(new Point3D(x + boundingBox.X,
                                y + 1 + boundingBox.Y, z + 1 + boundingBox.Z)))
                            cubeIndex |= 128;

                        // ルックアップテーブルを使用して三角形を生成
                        var triangles =
                            MarchingCubesLookupTable.GetTriangles(cubeIndex);
                        foreach (var triangle in triangles)
                        {
                            foreach (var edge in triangle)
                            {
                                var v = GetInterpolatedVertexPosition(edge, x,
                                    y, z, region, imageIntensities, isoValue,
                                    totalWidth, boundingBox);
                                mesh.Positions.Add(v);
                                mesh.TriangleIndices.Add(mesh.Positions.Count -
                                    1);
                            }
                        }

                        processedVoxels++;
                        if (processedVoxels % 1000 == 0 ||
                            processedVoxels == totalVoxels)
                        {
                            double progressValue = (double)processedVoxels /
                                totalVoxels * 100;
                            progress.Report(((int)progressValue,
                                $"サーフェスモデル(線形補間)を生成中...\n" +
                                $"処理済みボクセル数: {processedVoxels}/{totalVoxels}\n" +
                                $"生成されたポイント数: {mesh.Positions.Count}\n" +
                                $"生成された三角形の数: {mesh.TriangleIndices.Count / 3}"));
                        }
                    }
                }
            }

            return mesh;
        }

        private Point3D GetInterpolatedVertexPosition(int edge, int x, int y,
            int z, BloodVessel3DRegion region, double[,,] imageIntensities,
            double isoValue, int totalWidth,
            (int X, int Y, int Z, int Width, int Height, int Depth) boundingBox)
        {
            int x1, y1, z1, x2, y2, z2;
            MarchingCubesLookupTable.GetEdgeEndpoints(edge, x, y, z, out x1,
                out y1, out z1, out x2, out y2, out z2);

            // バウンディングボックスの位置を考慮して座標を調整
            x1 += boundingBox.X;
            x2 += boundingBox.X;
            y1 += boundingBox.Y;
            y2 += boundingBox.Y;
            z1 += boundingBox.Z;
            z2 += boundingBox.Z;

            double v1 = imageIntensities[x1, y1, z1];
            double v2 = imageIntensities[x2, y2, z2];
            double mu = GetInterpolationFactor(v1, v2, isoValue);

            // X座標を反転
            double invertedX1 = totalWidth - 1 - x1;
            double invertedX2 = totalWidth - 1 - x2;

            return new Point3D(invertedX1 + mu * (invertedX2 - invertedX1),
                y1 + mu * (y2 - y1), z1 + mu * (z2 - z1));
        }

        private double GetInterpolationFactor(double v1, double v2,
            double isoValue)
        {
            if (Math.Abs(v1 - v2) < 0.00001)
                return 0.5;

            return (isoValue - v1) / (v2 - v1);
        }

        private MaterialGroup CreateMaterial()
        {
            var materialGroup = new MaterialGroup();

            // 拡散反射（基本的な色と陰影）
            materialGroup.Children.Add(
                new DiffuseMaterial(
                    new SolidColorBrush(Color.FromRgb(200, 0, 0))));

            // 鏡面反射（ハイライト）
            materialGroup.Children.Add(new SpecularMaterial(
                new SolidColorBrush(Color.FromArgb(100, 255, 100, 100)), 10));

            return materialGroup;
        }

        private double[,,] GetImageIntensities(FileManager fileManager,
            (int X, int Y, int Z, int Width, int Height, int Depth) boundingBox)
        {
            var intensities = new double[boundingBox.Width, boundingBox.Height,
                boundingBox.Depth];

            for (int z = 0; z < boundingBox.Depth; z++)
            {
                var dicomFile = fileManager.DicomFiles[z + boundingBox.Z];
                var image = dicomFile.GetImage();
                var renderedImage = image.RenderImage()
                    .As<System.Windows.Media.Imaging.WriteableBitmap>();
                var stride =
                    renderedImage.PixelWidth * 4; // 4 bytes per pixel (BGRA)
                var pixels = new byte[renderedImage.PixelHeight * stride];
                renderedImage.CopyPixels(pixels, stride, 0);

                for (int y = 0; y < boundingBox.Height; y++)
                {
                    for (int x = 0; x < boundingBox.Width; x++)
                    {
                        int index = ((y + boundingBox.Y) * stride) +
                                    ((x + boundingBox.X) * 4);
                        byte intensity = pixels[index]; // Blue channel
                        intensity = Math.Max(_intensityMin,
                            Math.Min(_intensityMax, intensity));
                        intensities[x, y, z] = (intensity - _intensityMin) /
                                               (double)(_intensityMax -
                                                   _intensityMin);
                    }
                }
            }

            return intensities;
        }
    }
}
