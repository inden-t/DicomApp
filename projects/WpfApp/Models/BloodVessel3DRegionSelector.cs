using System.Windows.Media.Media3D;
using Point = System.Drawing.Point;

namespace DicomApp.Models
{
    public class BloodVessel3DRegionSelector
    {
        private readonly FileManager _fileManager;
        private readonly BloodVessel3DRegion _selectedRegion;

        private List<byte[]> _renderedImages = new();
        private int _imageWidth;
        private int _imageHeight;

        private List<HashSet<(int X, int Y, int Z)>> _selectionHistory = new();

        private int _currentHistoryIndex = -1;

        public BloodVessel3DRegionSelector(FileManager fileManager)
        {
            _fileManager = fileManager;
            _selectedRegion = new BloodVessel3DRegion();
            SaveCurrentState();
        }

        private void SaveCurrentState()
        {
            if (_currentHistoryIndex < _selectionHistory.Count - 1)
            {
                _selectionHistory.RemoveRange(_currentHistoryIndex + 1,
                    _selectionHistory.Count - _currentHistoryIndex - 1);
            }

            _selectionHistory.Add(
                new HashSet<(int X, int Y, int Z)>(_selectedRegion
                    .SelectedVoxels));
            _currentHistoryIndex = _selectionHistory.Count - 1;
        }

        public bool CanUndo()
        {
            return _currentHistoryIndex > 0;
        }

        public void Undo()
        {
            if (CanUndo())
            {
                _currentHistoryIndex--;
                _selectedRegion.SelectedVoxels =
                    new HashSet<(int X, int Y, int Z)>(
                        _selectionHistory[_currentHistoryIndex]);
            }
        }

        public void PreRenderImages()
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
            }
        }

        // 3D塗りつぶし選択の実装
        public void Select3DRegion(Point3D seedPoint, int threshold)
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
                queue.Enqueue(new Point3D(seedX, seedY, seedZ));
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
            }

            SaveCurrentState();
        }

        private void CheckAndEnqueueNeighbor(int x, int y, int z, int width,
            int height, int depth, bool[,,] visited, Queue<Point3D> queue)
        {
            if (IsValidPoint(x, y, z, width, height, depth) &&
                !visited[x, y, z])
            {
                queue.Enqueue(new Point3D(x, y, z));
                visited[x, y, z] = true;
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

        public void Select2DRegion(Point3D seedPoint, int threshold)
        {
            if (_fileManager.DicomFiles.Count == 0)
            {
                return;
            }

            int x = (int)seedPoint.X;
            int y = (int)seedPoint.Y;
            int z = (int)seedPoint.Z;

            var dicomFile = _fileManager.DicomFiles[z];
            var image = dicomFile.GetImage();
            var renderedImage = image.RenderImage()
                .As<System.Windows.Media.Imaging.WriteableBitmap>();
            int width = renderedImage.PixelWidth;
            int height = renderedImage.PixelHeight;

            if (!IsValidPoint(x, y, z, width, height,
                    _fileManager.DicomFiles.Count))
            {
                return;
            }

            bool[,] visited = new bool[width, height];
            Queue<Point> queue = new Queue<Point>();

            queue.Enqueue(new Point(x, y));
            visited[x, y] = true;

            int stride = width * 4; // 4 bytes per pixel (BGRA)
            byte[] pixels = new byte[height * stride];
            renderedImage.CopyPixels(pixels, stride, 0);

            while (queue.Count > 0)
            {
                Point current = queue.Dequeue();
                x = current.X;
                y = current.Y;

                if (GetPixelIntensity(x, y, pixels, stride) > threshold)
                {
                    _selectedRegion.AddVoxel(new Point3D(x, y, z));

                    // 4方向の隣接ピクセルをチェック
                    CheckAndEnqueue2DNeighbor(x + 1, y, z, visited, queue,
                        width, height);
                    CheckAndEnqueue2DNeighbor(x - 1, y, z, visited, queue,
                        width, height);
                    CheckAndEnqueue2DNeighbor(x, y + 1, z, visited, queue,
                        width, height);
                    CheckAndEnqueue2DNeighbor(x, y - 1, z, visited, queue,
                        width, height);
                }
            }

            SaveCurrentState();
        }

        private void CheckAndEnqueue2DNeighbor(int x, int y, int z,
            bool[,] visited, Queue<Point> queue, int width, int height)
        {
            if (IsValidPoint(x, y, z, width, height,
                    _fileManager.DicomFiles.Count) && !visited[x, y])
            {
                queue.Enqueue(new Point(x, y));
                visited[x, y] = true;
            }
        }

        private byte GetPixelIntensity(int x, int y, byte[] pixels, int stride)
        {
            int index = (y * stride) + (x * 4);
            return pixels[index]; // Blue channel
        }

        public void Clear3DRegion(Point3D seedPoint)
        {
            int x = (int)seedPoint.X;
            int y = (int)seedPoint.Y;
            int z = (int)seedPoint.Z;

            // 3D領域のクリア処理を実装
            // 例: 6方向の塗りつぶしアルゴリズムを使用
            Queue<Point3D> queue = new Queue<Point3D>();
            queue.Enqueue(new Point3D(x, y, z));

            while (queue.Count > 0)
            {
                Point3D p = queue.Dequeue();
                x = (int)p.X;
                y = (int)p.Y;
                z = (int)p.Z;

                if (_selectedRegion.ContainsVoxel(p))
                {
                    _selectedRegion.RemoveVoxel(p);

                    queue.Enqueue(new Point3D(x - 1, y, z));
                    queue.Enqueue(new Point3D(x + 1, y, z));
                    queue.Enqueue(new Point3D(x, y - 1, z));
                    queue.Enqueue(new Point3D(x, y + 1, z));
                    queue.Enqueue(new Point3D(x, y, z - 1));
                    queue.Enqueue(new Point3D(x, y, z + 1));
                }
            }

            SaveCurrentState();
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

            SaveCurrentState();
        }

        // 選択領域の取得
        public BloodVessel3DRegion GetSelectedRegion()
        {
            // 選択された領域を返す
            return _selectedRegion;
        }
    }
}
