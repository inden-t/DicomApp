using System.Windows;
using DicomApp.UseCases;

namespace DicomApp.Views
{
    public class BloodVessel3DViewerFactory : IBloodVessel3DViewerFactory
    {
        public IBloodVessel3DViewer Create()
        {
            var viewer = new BloodVessel3DViewer();
            viewer.Owner = Application.Current.MainWindow;
            return viewer;
        }
    }
}
