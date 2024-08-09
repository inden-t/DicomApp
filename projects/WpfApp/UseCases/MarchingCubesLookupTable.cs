using System.Windows.Media.Media3D;

namespace DicomApp.Models
{
    public static class MarchingCubesLookupTable
    {
        private static readonly int[][] triangleTable = new int[256][];

        static MarchingCubesLookupTable()
        {
            // ここにTriangle Tableの初期化コードを記述します
            // 256のケースすべてを記述するのは長くなるため、一部のみ示します
            triangleTable[0] = new int[0];
            triangleTable[1] = new int[] { 0, 8, 3 };
            triangleTable[2] = new int[] { 0, 1, 9 };
            triangleTable[3] = new int[] { 1, 8, 3, 9, 8, 1 };
            // ... 他のケースも同様に初期化 ...
        }

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

        public static Point3D GetVertexPosition(int edge, int x, int y, int z)
        {
            switch (edge)
            {
                case 0: return new Point3D(x + 0.5, y, z);
                case 1: return new Point3D(x + 1, y + 0.5, z);
                case 2: return new Point3D(x + 0.5, y + 1, z);
                case 3: return new Point3D(x, y + 0.5, z);
                case 4: return new Point3D(x + 0.5, y, z + 1);
                case 5: return new Point3D(x + 1, y + 0.5, z + 1);
                case 6: return new Point3D(x + 0.5, y + 1, z + 1);
                case 7: return new Point3D(x, y + 0.5, z + 1);
                case 8: return new Point3D(x, y, z + 0.5);
                case 9: return new Point3D(x + 1, y, z + 0.5);
                case 10: return new Point3D(x + 1, y + 1, z + 0.5);
                case 11: return new Point3D(x, y + 1, z + 0.5);
                default: throw new ArgumentOutOfRangeException(nameof(edge));
            }
        }
    }
}