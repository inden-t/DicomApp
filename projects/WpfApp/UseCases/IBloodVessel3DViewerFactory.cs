using System.Windows.Media.Media3D;

namespace DicomApp.UseCases
{
    public interface IBloodVessel3DViewerFactory
    {
        IBloodVessel3DViewer Create();
    }
}
