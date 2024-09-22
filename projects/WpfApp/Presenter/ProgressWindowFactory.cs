using System.Windows;
using DicomApp.MainUseCases.PresenterInterface;
using DicomApp.WpfApp.Views;

namespace DicomApp.WpfApp.Presenter
{
    public class ProgressWindowFactory : IProgressWindowFactory
    {
        public IProgressWindow Create()
        {
            var progressWindow = new ProgressWindow();
            progressWindow.Owner = Application.Current.MainWindow;
            return progressWindow;
        }
    }
}
