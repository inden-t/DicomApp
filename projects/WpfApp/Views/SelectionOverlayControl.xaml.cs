using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DicomApp.ViewModels;

namespace DicomApp.Views
{
    public partial class SelectionOverlayControl : UserControl
    {
        public SelectionOverlayControl(
            SelectionOverlayControlViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
