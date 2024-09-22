using System.Windows;
using DicomApp.MainUseCases.PresenterInterface;
using DicomApp.WpfApp.Views;

namespace DicomApp.WpfApp.Presenter
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
