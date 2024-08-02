using System.Windows;
using DicomApp.UseCases;

namespace DicomApp.Views
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
