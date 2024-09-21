using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DicomApp.ViewModels;

namespace DicomApp.Views
{
    public partial class SelectionOverlayControl : UserControl
    {
        public SelectionOverlayControl()
        {
            InitializeComponent();
            DataContext = new SelectionOverlayControlViewModel();
        }

        public SelectionOverlayControlViewModel ViewModel =>
            (SelectionOverlayControlViewModel)DataContext;
    }
}
