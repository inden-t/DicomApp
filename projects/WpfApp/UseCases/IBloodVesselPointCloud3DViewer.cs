using System;
using System.Windows.Media.Media3D;

namespace DicomApp.UseCases
{
    public interface IBloodVesselPointCloud3DViewer
    {
        void SetModel(Model3DGroup model);
        void Show();
    }
}
