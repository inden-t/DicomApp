using System.Windows.Media;
using System.Windows.Media.Media3D;
using DicomApp.BloodVesselExtraction.Models;
using DicomApp.CoreModels.Models;
using DicomApp.MainUseCases.Algorithms;

namespace DicomApp.BloodVesselExtraction.UseCases
{
    public class BloodVesselSurfaceModelGenerator
    {
        private const byte _intensityMin = 0;
        private const byte _intensityMax = 255;

        public async Task<Model3DGroup> GenerateModelAsync(
            FileManager fileManager, BloodVessel3DRegion region, int threshold,
            int thresholdUpperLimit,
            IProgress<(int value, string text)> progress)
        {
            return await Task.Run(() => CreateSurfaceModel(fileManager, region,
                threshold, thresholdUpperLimit, progress));
        }

        private Model3DGroup CreateSurfaceModel(FileManager fileManager,
            BloodVessel3DRegion region, int threshold, int thresholdUpperLimit,
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
                imageIntensities, threshold, thresholdUpperLimit, progress,
                width);

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

        private MeshGeometry3D CreateSurfaceFromVoxels(
            BloodVessel3DRegion region, double[,,] imageIntensities,
            int threshold, int thresholdUpperLimit,
            IProgress<(int value, string text)> progress, int totalWidth)
        {
            var mesh = new MeshGeometry3D();
            var boundingBox = GetBoundingBox(region);
            int width = boundingBox.Width;
            int height = boundingBox.Height;
            int depth = boundingBox.Depth;

            int totalVoxels = (width - 1) * (height - 1) * (depth - 1);
            int processedVoxels = 0;

            double isoValueLower = threshold / 255.0; // 下限しきい値を0.0～1.0の範囲に正規化
            double isoValueUpper =
                thresholdUpperLimit / 255.0; // 上限しきい値を0.0～1.0の範囲に正規化

            for (int x = 0; x < width - 1; x++)
            {
                for (int y = 0; y < height - 1; y++)
                {
                    for (int z = 0; z < depth - 1; z++)
                    {
                        // Marching Cubesアルゴリズムの実装
                        int cubeIndex = 0;
                        if (IsVoxelInRange(x, y, z, boundingBox, region))
                            cubeIndex |= 1;
                        if (IsVoxelInRange(x + 1, y, z, boundingBox, region))
                            cubeIndex |= 2;
                        if (IsVoxelInRange(x + 1, y + 1, z, boundingBox,
                                region))
                            cubeIndex |= 4;
                        if (IsVoxelInRange(x, y + 1, z, boundingBox, region))
                            cubeIndex |= 8;
                        if (IsVoxelInRange(x, y, z + 1, boundingBox, region))
                            cubeIndex |= 16;
                        if (IsVoxelInRange(x + 1, y, z + 1, boundingBox,
                                region))
                            cubeIndex |= 32;
                        if (IsVoxelInRange(x + 1, y + 1, z + 1, boundingBox,
                                region))
                            cubeIndex |= 64;
                        if (IsVoxelInRange(x, y + 1, z + 1, boundingBox,
                                region))
                            cubeIndex |= 128;

                        // ルックアップテーブルを使用して三角形を生成
                        var triangles =
                            MarchingCubesLookupTable.GetTriangles(cubeIndex);
                        foreach (var triangle in triangles)
                        {
                            foreach (var edge in triangle)
                            {
                                var v = GetInterpolatedVertexPosition(edge, x,
                                    y, z, region, imageIntensities,
                                    isoValueLower, isoValueUpper,
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

        private bool IsVoxelInRange(int x, int y, int z,
            (int X, int Y, int Z, int Width, int Height, int Depth) boundingBox,
            BloodVessel3DRegion region)
        {
            return region.ContainsVoxel(new Point3D(x + boundingBox.X,
                y + boundingBox.Y, z + boundingBox.Z));
        }

        private Point3D GetInterpolatedVertexPosition(int edge, int x, int y,
            int z, BloodVessel3DRegion region, double[,,] imageIntensities,
            double isoValueLower, double isoValueUpper, int totalWidth,
            (int X, int Y, int Z, int Width, int Height, int Depth) boundingBox)
        {
            int x1, y1, z1, x2, y2, z2;
            MarchingCubesLookupTable.GetEdgeEndpoints(edge, x, y, z, out x1,
                out y1, out z1, out x2, out y2, out z2);

            // imageIntensities配列用の相対座標を計算
            int rx1 = x1, ry1 = y1, rz1 = z1;
            int rx2 = x2, ry2 = y2, rz2 = z2;

            // グローバル座標系での位置を計算
            x1 += boundingBox.X;
            x2 += boundingBox.X;
            y1 += boundingBox.Y;
            y2 += boundingBox.Y;
            z1 += boundingBox.Z;
            z2 += boundingBox.Z;

            double v1 = imageIntensities[rx1, ry1, rz1];
            double v2 = imageIntensities[rx2, ry2, rz2];
            double mu =
                GetInterpolationFactor(v1, v2, isoValueLower, isoValueUpper);

            // X座標を反転
            double invertedX1 = totalWidth - 1 - x1;
            double invertedX2 = totalWidth - 1 - x2;

            return new Point3D(invertedX1 + mu * (invertedX2 - invertedX1),
                y1 + mu * (y2 - y1), z1 + mu * (z2 - z1));
        }

        private double GetInterpolationFactor(double v1, double v2,
            double isoValueLower, double isoValueUpper)
        {
            // v1とv2の値がほぼ等しい場合
            if (Math.Abs(v1 - v2) < 0.00001)
                return 0.5;

            // v1とv2が両方ともしきい値の範囲外にある場合
            if ((v1 < isoValueLower || v1 > isoValueUpper) &&
                (v2 < isoValueLower || v2 > isoValueUpper))
                return 0.5;

            // 片方がしきい値の下限を下回る場合
            if (v1 < isoValueLower || v2 < isoValueLower)
                return (isoValueLower - v1) / (v2 - v1);

            // 片方がしきい値の上限を上回る場合
            if (v1 > isoValueUpper || v2 > isoValueUpper)
                return (v1 - isoValueUpper) / (v1 - v2);

            // v1とv2が両方ともしきい値の範囲内にある場合
            return 0.5;
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
