using System;
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
            // ここでは簡単な実装例として、点群を直接結んでサーフェスを作成します
            // 実際のアプリケーションでは、より高度なアルゴリズム（例：Marching Cubes）を使用することをお勧めします

            var mesh = new MeshGeometry3D();

            for (int i = 0; i < points.Count - 1; i++)
            {
                mesh.Positions.Add(points[i]);
                mesh.Positions.Add(points[i + 1]);

                mesh.TriangleIndices.Add(i * 2);
                mesh.TriangleIndices.Add(i * 2 + 1);
                mesh.TriangleIndices.Add(i * 2 + 2);
            }

            return mesh;
        }
    }
}
