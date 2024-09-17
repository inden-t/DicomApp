using System.Drawing;
using System.Windows.Media.Media3D;

namespace DicomApp.Models
{
    public class BloodVessel3DRegionSelector
    {
        private readonly FileManager _fileManager;
        private readonly BloodVessel3DRegion _selectedRegion;

        private int _visitedCount;
        int _visitedXMax = 0;
        int _visitedXMin = 0;
        int _visitedYMax = 0;
        int _visitedYMin = 0;
        int _visitedZMax = 0;
        int _visitedZMin = 0;

        private List<byte[]> _renderedImages;
        private int _imageWidth;
        private int _imageHeight;

        public BloodVessel3DRegionSelector(FileManager fileManager)
        {
            _fileManager = fileManager;
            _selectedRegion = new BloodVessel3DRegion();
        }

        private void PreRenderImages(
            IProgress<(int value, string text)> progress)
        {
            _renderedImages = new List<byte[]>();
            int index = 0;
            foreach (var dicomFile in _fileManager.DicomFiles)
            {
                index++;
                var image = dicomFile.GetImage();
                var renderedImage = image.RenderImage()
                    .As<System.Windows.Media.Imaging.WriteableBitmap>();
                _imageWidth = renderedImage.PixelWidth;
                _imageHeight = renderedImage.PixelHeight;
                var stride = _imageWidth * 4; // 4 bytes per pixel (BGRA)
                var pixels = new byte[_imageHeight * stride];
                renderedImage.CopyPixels(pixels, stride, 0);
                _renderedImages.Add(pixels);

                int progressValue = index / _fileManager.DicomFiles.Count;
                string progressText = $"3次元塗りつぶし選択を実行中...\n" +
                                      $"プリレンダリング中... ({index} / {_fileManager.DicomFiles.Count})\n";

                progress.Report((progressValue, progressText));
            }
        }

        // 3D塗りつぶし選択の実装
        public void Select3DRegion(Point3D seedPoint, int threshold,
            IProgress<(int value, string text)> progress)
        {
            if (_fileManager.DicomFiles.Count == 0)
            {
                return;
            }

            PreRenderImages(progress);

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

            _visitedXMax = seedX;
            _visitedXMin = seedX;
            _visitedYMax = seedY;
            _visitedYMin = seedY;
            _visitedZMax = seedZ;
            _visitedZMin = seedZ;

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

                long progressValue = (long)(_visitedXMax - _visitedXMin + 1) *
                                     (_visitedYMax - _visitedYMin + 1) *
                                     (_visitedZMax - _visitedZMin + 1) * 100 /
                                     (width * height * depth);
                string progressText = $"3次元塗りつぶし選択を実行中...\n" +
                                      $"進捗: {progressValue}%\n" +
                                      $"点の個数: {_visitedCount}個";

                progress.Report(((int)progressValue, progressText));
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
            var stride = _imageWidth * 4;
            var pixels = _renderedImages[z];
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

        public void Clear2DRegion(Point3D seedPoint)
        {
            int x = (int)seedPoint.X;
            int y = (int)seedPoint.Y;
            int z = (int)seedPoint.Z;

            // 2D領域のクリア処理を実装
            // 例: 4方向の塗りつぶしアルゴリズムを使用
            Stack<Point> stack = new Stack<Point>();
            stack.Push(new Point(x, y));

            while (stack.Count > 0)
            {
                Point p = stack.Pop();
                x = p.X;
                y = p.Y;

                if (_selectedRegion.ContainsVoxel(new Point3D(x, y, z)))
                {
                    _selectedRegion.RemoveVoxel(new Point3D(x, y, z));

                    if (x > 0) stack.Push(new Point(x - 1, y));
                    if (x < _imageWidth - 1) stack.Push(new Point(x + 1, y));
                    if (y > 0) stack.Push(new Point(x, y - 1));
                    if (y < _imageHeight - 1) stack.Push(new Point(x, y + 1));
                }
            }
        }

        public void Clear3DRegion(Point3D seedPoint,
            IProgress<(int value, string text)> progress)
        {
            int x = (int)seedPoint.X;
            int y = (int)seedPoint.Y;
            int z = (int)seedPoint.Z;

            // 3D領域のクリア処理を実装
            // 例: 6方向の塗りつぶしアルゴリズムを使用
            Queue<Point3D> queue = new Queue<Point3D>();
            queue.Enqueue(new Point3D(x, y, z));

            int clearedCount = 0;
            int totalVoxels = _selectedRegion.SelectedVoxels.Count;

            while (queue.Count > 0)
            {
                Point3D p = queue.Dequeue();
                x = (int)p.X;
                y = (int)p.Y;
                z = (int)p.Z;

                if (_selectedRegion.ContainsVoxel(p))
                {
                    _selectedRegion.RemoveVoxel(p);
                    clearedCount++;

                    queue.Enqueue(new Point3D(x - 1, y, z));
                    queue.Enqueue(new Point3D(x + 1, y, z));
                    queue.Enqueue(new Point3D(x, y - 1, z));
                    queue.Enqueue(new Point3D(x, y + 1, z));
                    queue.Enqueue(new Point3D(x, y, z - 1));
                    queue.Enqueue(new Point3D(x, y, z + 1));

                    if (clearedCount % 1000 == 0 || queue.Count == 0)
                    {
                        int progressValue =
                            (int)((double)clearedCount / totalVoxels * 100);
                        string progressText = $"3次元塗りつぶし選択を解除中...\n" +
                                              $"進捗: {progressValue}%\n" +
                                              $"クリアした点の個数: {clearedCount}個";

                        progress.Report((progressValue, progressText));
                    }
                }
            }
        }
    }
}
