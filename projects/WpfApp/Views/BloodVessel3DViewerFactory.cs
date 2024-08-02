using System.Windows;
using DicomApp.Views;

namespace DicomApp.UseCases
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
