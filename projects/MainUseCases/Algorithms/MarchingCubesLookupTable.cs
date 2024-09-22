using System.Windows.Media.Media3D;

namespace DicomApp.MainUseCases.Algorithms
{
    public static partial class MarchingCubesLookupTable
    {
        public static int[][] GetTriangles(int cubeIndex)
        {
            if (cubeIndex < 0 || cubeIndex >= 256)
            {
                throw new ArgumentOutOfRangeException(nameof(cubeIndex));
            }

            int[] triangles = triangleTable[cubeIndex];
            int triangleCount = triangles.Length / 3;
            int[][] result = new int[triangleCount][];

            for (int i = 0; i < triangleCount; i++)
            {
                result[i] = new int[3];
                result[i][0] = triangles[i * 3];
                result[i][1] = triangles[i * 3 + 1];
                result[i][2] = triangles[i * 3 + 2];
            }

            return result;
        }

        public static void GetEdgeEndpoints(int edge, int x, int y, int z,
            out int x1, out int y1, out int z1, out int x2, out int y2,
            out int z2)
        {
            switch (edge)
            {
                case 0:
                    x1 = x;
                    y1 = y;
                    z1 = z;
                    x2 = x + 1;
                    y2 = y;
                    z2 = z;
                    break;
                case 1:
                    x1 = x + 1;
                    y1 = y;
                    z1 = z;
                    x2 = x + 1;
                    y2 = y + 1;
                    z2 = z;
                    break;
                case 2:
                    x1 = x + 1;
                    y1 = y + 1;
                    z1 = z;
                    x2 = x;
                    y2 = y + 1;
                    z2 = z;
                    break;
                case 3:
                    x1 = x;
                    y1 = y + 1;
                    z1 = z;
                    x2 = x;
                    y2 = y;
                    z2 = z;
                    break;
                case 4:
                    x1 = x;
                    y1 = y;
                    z1 = z + 1;
                    x2 = x + 1;
                    y2 = y;
                    z2 = z + 1;
                    break;
                case 5:
                    x1 = x + 1;
                    y1 = y;
                    z1 = z + 1;
                    x2 = x + 1;
                    y2 = y + 1;
                    z2 = z + 1;
                    break;
                case 6:
                    x1 = x + 1;
                    y1 = y + 1;
                    z1 = z + 1;
                    x2 = x;
                    y2 = y + 1;
                    z2 = z + 1;
                    break;
                case 7:
                    x1 = x;
                    y1 = y + 1;
                    z1 = z + 1;
                    x2 = x;
                    y2 = y;
                    z2 = z + 1;
                    break;
                case 8:
                    x1 = x;
                    y1 = y;
                    z1 = z;
                    x2 = x;
                    y2 = y;
                    z2 = z + 1;
                    break;
                case 9:
                    x1 = x + 1;
                    y1 = y;
                    z1 = z;
                    x2 = x + 1;
                    y2 = y;
                    z2 = z + 1;
                    break;
                case 10:
                    x1 = x + 1;
                    y1 = y + 1;
                    z1 = z;
                    x2 = x + 1;
                    y2 = y + 1;
                    z2 = z + 1;
                    break;
                case 11:
                    x1 = x;
                    y1 = y + 1;
                    z1 = z;
                    x2 = x;
                    y2 = y + 1;
                    z2 = z + 1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(edge));
            }
        }

        public static Point3D GetVertexPosition(int edge, int x, int y, int z,
            double interpolationFactor = 0.5)
        {
            double ipf = interpolationFactor;
            switch (edge)
            {
                case 0: return new Point3D(x + ipf, y, z);
                case 1: return new Point3D(x + 1, y + ipf, z);
                case 2: return new Point3D(x + 1 - ipf, y + 1, z);
                case 3: return new Point3D(x, y + 1 - ipf, z);
                case 4: return new Point3D(x + ipf, y, z + 1);
                case 5: return new Point3D(x + 1, y + ipf, z + 1);
                case 6: return new Point3D(x + 1 - ipf, y + 1, z + 1);
                case 7: return new Point3D(x, y + 1 - ipf, z + 1);
                case 8: return new Point3D(x, y, z + ipf);
                case 9: return new Point3D(x + 1, y, z + ipf);
                case 10: return new Point3D(x + 1, y + 1, z + ipf);
                case 11: return new Point3D(x, y + 1, z + ipf);
                default: throw new ArgumentOutOfRangeException(nameof(edge));
            }
        }
    }
}

/*
triangleTable 対応図

頂点の数値は cubeIndex のビット値
辺の数値は対応する edge の番号
  triangleTable[cubeIndex] = 三角メッシュの頂点が位置する edge の配列
  triangleTable[cubeIndex][頂点のindex] = edge
という関係

   128----6---64
   /|         /|
  7 |        5 |
 /  11      /  10
16----4---32   |
|   |      |   |
|   8 ---2-|- -4
8  /       9  /
| 3        | 1
|/         |/
1 ---0---- 2

z
| y
|/
O --- x

*/
