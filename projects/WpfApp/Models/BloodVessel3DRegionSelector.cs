using System.Windows.Media.Media3D;

namespace DicomApp.Models
{
    public class BloodVessel3DRegionSelector
    {
        private readonly FileManager _fileManager;
        private BloodVessel3DRegion _selectedRegion;

        private int _visitedCount;
        int _visitedXMax = 0;
        int _visitedXMin = 0;
        int _visitedYMax = 0;
        int _visitedYMin = 0;
        int _visitedZMax = 0;
        int _visitedZMin = 0;


        public BloodVessel3DRegionSelector(FileManager fileManager)
        {
            _fileManager = fileManager;
            _selectedRegion = new BloodVessel3DRegion();
        }

        // 3D塗りつぶし選択の実装
        public void Select3DRegion(Point3D seedPoint, int threshold,
            IProgress<(int value, int pointNum)> progress)
        {
            if (_fileManager.DicomFiles.Count == 0)
            {
                return;
            }

            int width = _fileManager.DicomFiles[0].GetImage().Width;
            int height = _fileManager.DicomFiles[0].GetImage().Height;
            int depth = _fileManager.DicomFiles.Count;

            bool[,,] visited = new bool[width, height, depth];
            Queue<Point3D> queue = new Queue<Point3D>();

            int seedX = (int)seedPoint.X;
            int seedY = (int)seedPoint.Y;
            int seedZ = (int)seedPoint.Z;

            if (IsValidPoint(seedX, seedY, seedZ, width, height, depth))
            {
                queue.Enqueue(seedPoint);
                visited[seedX, seedY, seedZ] = true;
            }

            while (queue.Count > 0)
            {
                Point3D current = queue.Dequeue();
                int x = (int)current.X;
                int y = (int)current.Y;
                int z = (int)current.Z;

                if (GetVoxelIntensity(x, y, z) > threshold)
                {
                    _selectedRegion.AddVoxel(current);

                    // 6方向の隣接ボクセルをチェック
                    CheckAndEnqueueNeighbor(x + 1, y, z, width, height, depth,
                        visited, queue);
                    CheckAndEnqueueNeighbor(x - 1, y, z, width, height, depth,
                        visited, queue);
                    CheckAndEnqueueNeighbor(x, y + 1, z, width, height, depth,
                        visited, queue);
                    CheckAndEnqueueNeighbor(x, y - 1, z, width, height, depth,
                        visited, queue);
                    CheckAndEnqueueNeighbor(x, y, z + 1, width, height, depth,
                        visited, queue);
                    CheckAndEnqueueNeighbor(x, y, z - 1, width, height, depth,
                        visited, queue);
                }

                int progressValue = (_visitedXMax - _visitedXMin + 1) *
                                    (_visitedYMax - _visitedYMin + 1) *
                                    (_visitedZMax - _visitedZMin + 1) * 100 /
                                    (width * height * depth);

                progress.Report((progressValue, _visitedCount));
            }
        }

        private void CheckAndEnqueueNeighbor(int x, int y, int z, int width,
            int height, int depth, bool[,,] visited, Queue<Point3D> queue)
        {
            if (IsValidPoint(x, y, z, width, height, depth) &&
                !visited[x, y, z])
            {
                queue.Enqueue(new Point3D(x, y, z));
                visited[x, y, z] = true;
                _visitedXMax = Math.Max(_visitedXMax, x);
                _visitedXMin = Math.Min(_visitedXMin, x);
                _visitedYMax = Math.Max(_visitedYMax, y);
                _visitedYMin = Math.Min(_visitedYMin, y);
                _visitedZMax = Math.Max(_visitedZMax, z);
                _visitedZMin = Math.Min(_visitedZMin, z);
                _visitedCount++;
            }
        }

        private bool IsValidPoint(int x, int y, int z, int width, int height,
            int depth)
        {
            return x >= 0 && x < width && y >= 0 && y < height && z >= 0 &&
                   z < depth;
        }

        private byte GetVoxelIntensity(int x, int y, int z)
        {
            var dicomFile = _fileManager.DicomFiles[z];
            var image = dicomFile.GetImage();
            var renderedImage = image.RenderImage()
                .As<System.Windows.Media.Imaging.WriteableBitmap>();
            var stride =
                renderedImage.PixelWidth * 4; // 4 bytes per pixel (BGRA)
            var pixels = new byte[renderedImage.PixelHeight * stride];
            renderedImage.CopyPixels(pixels, stride, 0);

            int index = (y * stride) + (x * 4);
            return pixels[index]; // Blue channel
        }

        // 選択領域の編集機能
        public void EditRegion(Point3D point, bool isAdd)
        {
            // 領域の追加または削除のロジックを実装
        }

        // 選択領域の取得
        public BloodVessel3DRegion GetSelectedRegion()
        {
            // 選択された領域を返す
            return _selectedRegion;
        }
    }
}
