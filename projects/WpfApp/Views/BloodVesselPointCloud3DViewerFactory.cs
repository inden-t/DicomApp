using System.Windows;
using DicomApp.UseCases;

namespace DicomApp.Views
{
    public class BloodVessel3DViewerFactory : IBloodVessel3DViewerFactory
    {
        public IBloodVesselPointCloud3DViewer Create()
        {
            var viewer = new BloodVesselPointCloud3DViewer();
            viewer.Owner = Application.Current.MainWindow;
            return viewer;
        }
    }
}
