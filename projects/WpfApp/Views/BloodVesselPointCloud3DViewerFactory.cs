using System.Windows;
using DicomApp.UseCases;

namespace DicomApp.Views
{
    public class
        BloodVesselPointCloud3DViewerFactory :
        IBloodVesselPointCloud3DViewerFactory
    {
        public IBloodVesselPointCloud3DViewer Create()
        {
            var viewer = new BloodVesselPointCloud3DViewer();
            viewer.Owner = Application.Current.MainWindow;
            return viewer;
        }
    }
}
