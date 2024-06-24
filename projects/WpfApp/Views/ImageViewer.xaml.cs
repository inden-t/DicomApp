using System;
using System.Windows.Controls;
using DicomApp.ViewModels;

namespace DicomApp.Views
{
    public partial class ImageViewer : UserControl
    {
        private readonly ImageViewerViewModel _viewModel;

        public ImageViewer(ImageViewerViewModel imageViewerViewModel)
        {
            InitializeComponent();

            _viewModel = imageViewerViewModel;
            DataContext = imageViewerViewModel;
        }

        private void UserControl_MouseWheel(object sender,
            System.Windows.Input.MouseWheelEventArgs e)
        {
            int offset = Math.Sign(-e.Delta);
            _viewModel.SwitchImageByOffset(offset);
        }
    }
}
