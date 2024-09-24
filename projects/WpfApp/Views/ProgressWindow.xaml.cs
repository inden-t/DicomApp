using System.Windows;
using System.Windows.Input;
using DicomApp.MainUseCases.PresenterInterface;
using DicomApp.WpfApp.ViewModels;

namespace DicomApp.WpfApp.Views
{
    public partial class ProgressWindow : Window, IProgressWindow
    {
        public ProgressWindowViewModel ViewModel { get; }

        public ProgressWindow()
        {
            InitializeComponent();

            ViewModel = new ProgressWindowViewModel();
            DataContext = ViewModel;
        }

        public void Start()
        {
            Application.Current.MainWindow!.IsEnabled = false;
            Mouse.OverrideCursor = Cursors.Wait;
            Show();
        }

        public void End()
        {
            Close();
            Mouse.OverrideCursor = null;
            Application.Current.MainWindow!.IsEnabled = true;
        }

        public void SetWindowTitle(string text)
        {
            ViewModel.WindowTitle.Value = text;
        }

        public void SetStatusText(string text)
        {
            ViewModel.StatusText.Value = text;
        }

        public void SetProgress(double progress)
        {
            ViewModel.Progress.Value = progress;
        }
    }
}
