using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using DicomApp.Models;
using FellowOakDicom;
using FellowOakDicom.Imaging;

namespace DicomApp.UseCases
{
    public class MakeBloodVessel3DUseCase
    {
        private readonly FileManager _fileManager;

        public MakeBloodVessel3DUseCase(FileManager fileManager)
        {
            _fileManager = fileManager;
        }

        public Model3DGroup Execute()
        {
            var model3DGroup = new Model3DGroup();

            foreach (var dicomFile in _fileManager.DicomFiles)
            {
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
                            var point = new Point3D(x, y,
                                _fileManager.DicomFiles.IndexOf(dicomFile));
                            var sphere = CreateSphere(point, 0.5);
                            model3DGroup.Children.Add(sphere);
                        }
                    }
                }
            }

            return model3DGroup;
        }

        private GeometryModel3D CreateSphere(Point3D center, double radius)
        {
            var sphere = new SphereBuilder();
            sphere.Center = center;
            sphere.Radius = radius;
            var geometry = sphere.ToMesh();

            var material =
                new DiffuseMaterial(System.Windows.Media.Brushes.Red);

            return new GeometryModel3D(geometry, material);
        }
    }

    // 簡単な球体を作成するヘルパークラス
    public class SphereBuilder
    {
        public Point3D Center { get; set; }
        public double Radius { get; set; }

        public MeshGeometry3D ToMesh(int thetaDiv = 10, int phiDiv = 10)
        {
            var positions = new Point3DCollection();
            var indices = new Int32Collection();

            for (int i = 0; i <= thetaDiv; i++)
            {
                double theta = i * Math.PI / thetaDiv;
                double sinTheta = Math.Sin(theta);
                double cosTheta = Math.Cos(theta);

                for (int j = 0; j <= phiDiv; j++)
                {
                    double phi = j * 2 * Math.PI / phiDiv;
                    double sinPhi = Math.Sin(phi);
                    double cosPhi = Math.Cos(phi);

                    double x = Center.X + Radius * sinTheta * cosPhi;
                    double y = Center.Y + Radius * sinTheta * sinPhi;
                    double z = Center.Z + Radius * cosTheta;

                    positions.Add(new Point3D(x, y, z));
                }
            }

            for (int i = 0; i < thetaDiv; i++)
            {
                for (int j = 0; j < phiDiv; j++)
                {
                    int first = (i * (phiDiv + 1)) + j;
                    int second = first + phiDiv + 1;

                    indices.Add(first);
                    indices.Add(second);
                    indices.Add(first + 1);

                    indices.Add(second);
                    indices.Add(second + 1);
                    indices.Add(first + 1);
                }
            }

            var mesh = new MeshGeometry3D();
            mesh.Positions = positions;
            mesh.TriangleIndices = indices;

            return mesh;
        }
    }
}
