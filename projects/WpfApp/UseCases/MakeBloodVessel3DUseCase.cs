using System;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using DicomApp.Models;

namespace DicomApp.UseCases
{
    public class MakeBloodVessel3DUseCase
    {
        private readonly FileManager _fileManager;
        private readonly IBloodVessel3DViewer _viewer;
        private readonly IProgressWindow _progressWindow;

        public MakeBloodVessel3DUseCase(FileManager fileManager,
            IBloodVessel3DViewer viewer,
            IProgressWindow progressWindow)
        {
            _fileManager = fileManager;
            _viewer = viewer;
            _progressWindow = progressWindow;
        }

        public async Task ExecuteAsync()
        {
            Mouse.OverrideCursor = Cursors.Wait;

            _progressWindow.Start();
            _progressWindow.SetStatusText("3Dモデルを生成中...");

            var model3DGroup = await Task.Run(() => CreateBloodVessel3DModel());

            _viewer.SetModel(model3DGroup);

            _progressWindow.End();
            _viewer.Show();

            Mouse.OverrideCursor = null;
        }

        private Model3DGroup CreateBloodVessel3DModel()
        {
            var model3DGroup = new Model3DGroup();
            int totalFiles = _fileManager.DicomFiles.Count;
            int totalSpheres = 0; // 追加された球体の総数をカウントする変数

            for (int i = 0; i < totalFiles; i++)
            {
                var dicomFile = _fileManager.DicomFiles[i];
                var image = dicomFile.GetImage();
                var renderedImage = image.RenderImage().As<WriteableBitmap>();
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
                            // x座標を反転させる
                            var point = new Point3D(width - 1 - x, y, i);
                            var sphere = CreateSphere(point, 0.5);
                            model3DGroup.Children.Add(sphere);
                            totalSpheres++; // 球体が追加されるたびにカウントを増やす
                        }
                    }
                }

                double progress = (i + 1) / (double)totalFiles * 100;
                _progressWindow.SetProgress(progress);
                _progressWindow.SetStatusText(
                    $"3Dモデルを生成中... ({i + 1}/{totalFiles} files)\n球体数: {totalSpheres}個");
            }

            model3DGroup.Freeze();

            return model3DGroup;
        }

        private GeometryModel3D CreateSphere(Point3D center, double radius)
        {
            var sphere = new SphereBuilder();
            sphere.Center = center;
            sphere.Radius = radius;
            var geometry = sphere.ToMesh();

            var material = new MaterialGroup();
            material.Children.Add(
                new DiffuseMaterial(new SolidColorBrush(Colors.Red)));
            material.Children.Add(
                new SpecularMaterial(new SolidColorBrush(Colors.White), 50));

            return new GeometryModel3D(geometry, material);
        }
    }
}
