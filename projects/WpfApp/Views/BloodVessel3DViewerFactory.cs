using System.Windows.Media.Media3D;
using DicomApp.Views;

namespace DicomApp.UseCases
{
    public class BloodVessel3DViewerFactory : IBloodVessel3DViewerFactory
    {
        public IBloodVessel3DViewer Create()
        {
            return new BloodVessel3DViewer();
        }
    }
}
