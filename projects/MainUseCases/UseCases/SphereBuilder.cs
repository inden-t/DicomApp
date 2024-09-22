using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace DicomApp.MainUseCases.UseCases
{
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
