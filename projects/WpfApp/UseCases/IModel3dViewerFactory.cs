using System.Windows.Media.Media3D;

namespace DicomApp.UseCases
{
    public interface IModel3dViewerFactory
    {
        IModel3dViewer Create();
    }
}
