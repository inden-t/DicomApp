using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DicomApp.ViewModels;

namespace DicomApp.Views
{
    public partial class SelectionOverlayControl : UserControl
    {
        private SelectionOverlayControlViewModel _viewModel;

        public SelectionOverlayControl(
            SelectionOverlayControlViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            _viewModel = viewModel;
        }

        private void OverlayImage_Click(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point mousePos = e.GetPosition(OverlayImage);
                double relativeX = mousePos.X / OverlayImage.ActualWidth;
                double relativeY = mousePos.Y / OverlayImage.ActualHeight;
                Mouse.OverrideCursor = Cursors.Wait;
                _viewModel.OnClick(relativeX, relativeY);
                Mouse.OverrideCursor = null;
            }
        }
    }
}
