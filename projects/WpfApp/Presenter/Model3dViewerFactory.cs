using System.Windows;
using DicomApp.UseCases;
using DicomApp.Views;

namespace DicomApp.Presenter
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
