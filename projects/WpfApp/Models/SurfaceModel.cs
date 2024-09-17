using System.Windows.Media.Media3D;

namespace DicomApp.Models
{
    public class SurfaceModel
    {
        public Model3DGroup ModelGroup { get; private set; }

        public SurfaceModel()
        {
            ModelGroup = new Model3DGroup();
        }

        public void AddMesh(MeshGeometry3D mesh, Material material)
        {
            var geometryModel = new GeometryModel3D(mesh, material);
            ModelGroup.Children.Add(geometryModel);
        }

        public void Clear()
        {
            ModelGroup.Children.Clear();
        }
    }
}