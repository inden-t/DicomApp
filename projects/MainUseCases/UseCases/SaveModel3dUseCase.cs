using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Media.Media3D;
using DicomApp.MainUseCases.PresenterInterface;

namespace DicomApp.MainUseCases.UseCases
{
    public class SaveModel3dUseCase
    {
        public void Execute(Model3DGroup model)
        {
            if (model == null)
            {
                throw new InvalidOperationException("現在のモデルが存在しません。");
            }

            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "3Dモデルファイル (*.obj)|*.obj|すべてのファイル (*.*)|*.*",
                DefaultExt = ".obj"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;
                try
                {
                    SaveModelToFile(model, filePath);
                    MessageBox.Show($"モデルを {filePath} に保存しました。", "保存完了",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"モデルの保存中にエラーが発生しました: {ex.Message}", "エラー",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SaveModelToFile(Model3DGroup model, string filePath)
        {
            // 簡単な.objファイル形式でモデルを保存します

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (var model3D in model.Children)
                {
                    if (model3D is GeometryModel3D geometryModel)
                    {
                        if (geometryModel.Geometry is MeshGeometry3D
                            mesh)
                        {
                            // 頂点を書き込む
                            foreach (Point3D point in mesh.Positions)
                            {
                                writer.WriteLine(
                                    $"v {point.X} {point.Y} {point.Z}");
                            }

                            // 面を書き込む
                            for (int i = 0;
                                 i < mesh.TriangleIndices.Count;
                                 i += 3)
                            {
                                writer.WriteLine(
                                    $"f {mesh.TriangleIndices[i] + 1} {mesh.TriangleIndices[i + 1] + 1} {mesh.TriangleIndices[i + 2] + 1}");
                            }
                        }
                    }
                }
            }
        }
    }
}
