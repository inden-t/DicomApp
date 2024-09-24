using System.Windows;
using DicomApp.MainUseCases.PresenterInterface;
using DicomApp.WpfApp.Views;

namespace DicomApp.WpfApp.Presenter
{
    public class Model3dViewerFactory : IModel3dViewerFactory
    {
        private readonly IProgressWindowFactory _progressWindowFactory;

        public Model3dViewerFactory(
            IProgressWindowFactory progressWindowFactory)
        {
            _progressWindowFactory = progressWindowFactory;
        }

        public IModel3dViewer Create()
        {
            var viewer = new Model3dViewer(_progressWindowFactory);
            viewer.Owner = Application.Current.MainWindow;
            return viewer;
        }
    }
}
