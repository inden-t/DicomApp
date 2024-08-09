using System.Windows.Media.Media3D;

namespace DicomApp.UseCases
{
    public interface IBloodVesselPointCloud3DViewerFactory
    {
        IBloodVesselPointCloud3DViewer Create();
    }
}
