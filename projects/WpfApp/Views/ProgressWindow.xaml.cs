using System.Windows;
using DicomApp.ViewModels;
using Reactive.Bindings;

namespace DicomApp.Views
{
    public partial class ProgressWindow : Window
    {
        public ProgressWindowViewModel ViewModel { get; }

        public ProgressWindow()
        {
            InitializeComponent();

            ViewModel = new ProgressWindowViewModel();
            DataContext = ViewModel;
        }
    }
}
