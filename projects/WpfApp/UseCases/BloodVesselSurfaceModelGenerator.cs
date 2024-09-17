using System;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using DicomApp.Models;

namespace DicomApp.UseCases
{
    public class BloodVesselSurfaceModelGenerator
    {
        public async Task<SurfaceModel> GenerateModelAsync(
            BloodVessel3DRegion region, int threshold
            //, IProgress<int> progress
        )
        {
            return await Task.Run(() =>
            {
                var surfaceModel = new SurfaceModel();
                var mesh = new MeshGeometry3D();

                // マーチングキューブ法を使用してメッシュを生成
                GenerateMeshUsingMarchingCubes(region, threshold, mesh
                    //, progress
                );

                // メッシュにマテリアルを適用
                var material =
                    new DiffuseMaterial(new SolidColorBrush(Colors.Red));
                surfaceModel.AddMesh(mesh, material);

                return surfaceModel;
            });
        }

        private void GenerateMeshUsingMarchingCubes(BloodVessel3DRegion region,
            int threshold, MeshGeometry3D mesh
            //, IProgress<int> progress
        )
        {
            // マーチングキューブ法の実装
            // この部分は複雑なアルゴリズムを含むため、ここでは簡略化しています
            // 実際の実装では、選択された領域からボクセルデータを取得し、
            // マーチングキューブ法を使用してメッシュを生成します

            // 進捗状況の報告
            for (int i = 0; i <= 100; i++)
            {
                //progress?.Report(i);
                System.Threading.Thread.Sleep(50); // シミュレーション用の遅延
            }

            // ダミーのメッシュデータを生成（実際の実装ではこの部分を置き換えてください）
            mesh.Positions.Add(new Point3D(0, 0, 0));
            mesh.Positions.Add(new Point3D(1, 0, 0));
            mesh.Positions.Add(new Point3D(0, 1, 0));
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(2);
        }
    }
}
