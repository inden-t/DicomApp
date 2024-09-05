using System.Windows;
using DicomApp.UseCases;

namespace DicomApp.Views
{
    public class Model3dViewerFactory : IModel3dViewerFactory
    {
        public IModel3dViewer Create()
        {
            var viewer = new Model3dViewer();
            viewer.Owner = Application.Current.MainWindow;
            return viewer;
        }
    }
}
