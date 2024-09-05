using System.Windows;
using DicomApp.UseCases;

namespace DicomApp.Views
{
    public class
        BloodVesselPointCloud3DViewerFactory :
        IBloodVesselPointCloud3DViewerFactory
    {
        public IModel3dViewer Create()
        {
            var viewer = new Model3dViewer();
            viewer.Owner = Application.Current.MainWindow;
            return viewer;
        }
    }
}
