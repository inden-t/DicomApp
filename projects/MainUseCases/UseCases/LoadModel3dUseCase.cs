using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using DicomApp.MainUseCases.PresenterInterface;

namespace DicomApp.MainUseCases.UseCases
{
    public class LoadModel3dUseCase
    {
        private readonly IModel3dViewer _viewer;

        public LoadModel3dUseCase(IModel3dViewer viewer)
        {
            _viewer = viewer;
        }

        public async Task Execute()
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "3Dモデルファイル (*.obj)|*.obj|すべてのファイル (*.*)|*.*",
                DefaultExt = ".obj"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                try
                {
                    Model3DGroup loadedModel = LoadModelFromFile(filePath);
                    _viewer.SetModel(loadedModel);
                    _viewer.Show();
                    MessageBox.Show($"モデルを {filePath} から読み込みました。", "読み込み完了",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"モデルの読み込み中にエラーが発生しました: {ex.Message}",
                        "エラー",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private Model3DGroup LoadModelFromFile(string filePath)
        {
            var model = new Model3DGroup();
            using var reader = new StreamReader(filePath);
            var mesh = new MeshGeometry3D();

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                var parts = line.Split(' ');
                if (parts[0] == "v")
                {
                    mesh.Positions.Add(new Point3D(
                        double.Parse(parts[1]),
                        double.Parse(parts[2]),
                        double.Parse(parts[3])));
                }
                else if (parts[0] == "f")
                {
                    mesh.TriangleIndices.Add(int.Parse(parts[1]) - 1);
                    mesh.TriangleIndices.Add(int.Parse(parts[2]) - 1);
                    mesh.TriangleIndices.Add(int.Parse(parts[3]) - 1);
                }
            }

            var material =
                new DiffuseMaterial(new SolidColorBrush(Colors.Gray));
            var geometryModel = new GeometryModel3D(mesh, material);
            model.Children.Add(geometryModel);

            return model;
        }
    }
}
