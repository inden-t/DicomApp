﻿using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using DicomApp.CoreModels.Models;
using DicomApp.MainUseCases.PresenterInterface;

namespace DicomApp.MainUseCases.UseCases
{
    public class GeneratePointCloudUseCase
    {
        private readonly FileManager _fileManager;
        private readonly IModel3dViewerFactory _viewerFactory;
        private readonly IProgressWindowFactory _progressWindowFactory;

        private IModel3dViewer _viewer;
        private IProgressWindow _progressWindow;

        public GeneratePointCloudUseCase(FileManager fileManager,
            IModel3dViewerFactory viewerFactory,
            IProgressWindowFactory progressWindowFactory)
        {
            _fileManager = fileManager;
            _viewerFactory = viewerFactory;
            _progressWindowFactory = progressWindowFactory;
        }

        public async Task ExecuteAsync()
        {
            Mouse.OverrideCursor = Cursors.Wait;

            _progressWindow = _progressWindowFactory.Create();
            _progressWindow.SetWindowTitle("モデル生成中");
            _progressWindow.Start();
            _progressWindow.SetStatusText("点群モデルを生成中...");

            var model3DGroup = await Task.Run(() => CreatePointCloud3dModel());

            _viewer = _viewerFactory.Create();
            _viewer.SetModel(model3DGroup);

            _progressWindow.End();
            _viewer.Show();

            Mouse.OverrideCursor = null;
        }

        private Model3DGroup CreatePointCloud3dModel()
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

                        if (intensity > 200) // 高輝度のしきい値
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
                    $"点群モデルを生成中... ({i + 1}/{totalFiles} files)\n球体数: {totalSpheres}個");
            }

            model3DGroup.Freeze();

            return model3DGroup;
        }

        private GeometryModel3D CreateSphere(Point3D center, double radius)
        {
            var sphere = new SphereBuilder();
            sphere.Center = center;
            sphere.Radius = radius;
            var geometry = sphere.ToMesh(4, 4);

            var material = new MaterialGroup();
            material.Children.Add(
                new DiffuseMaterial(new SolidColorBrush(Colors.Red)));
            material.Children.Add(
                new SpecularMaterial(new SolidColorBrush(Colors.White), 50));

            return new GeometryModel3D(geometry, material);
        }
    }
}
