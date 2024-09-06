using System;
using System.Diagnostics;
using System.Windows;
using DicomApp.UseCases;
using DicomApp.ViewModels;

namespace DicomApp.Views
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
            Show();
        }

        public void End()
        {
            Close();
            Application.Current.MainWindow!.IsEnabled = true;
        }

        public void SetWindowTitle(string text)
        {
            ViewModel.WindowTitle.Value = text;
        }

        public void SetProgress(double progress)
        {
            ViewModel.Progress.Value = progress;
        }

        public void SetStatusText(string text)
        {
            ViewModel.StatusText.Value = text;
        }
    }
}
