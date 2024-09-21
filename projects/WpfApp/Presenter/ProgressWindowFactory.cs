using System.Windows;
using DicomApp.UseCases;
using DicomApp.Views;

namespace DicomApp.Presenter
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
