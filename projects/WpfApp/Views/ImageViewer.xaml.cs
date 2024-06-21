using System;
using System.Windows.Controls;
using DicomApp.ViewModels;

namespace DicomApp.Views
{
    public partial class ImageViewer : UserControl
    {
        public ImageViewer(ImageViewerViewModel imageViewerViewModel)
        {
            InitializeComponent();

            DataContext = imageViewerViewModel;
        }

        private void UserControl_MouseWheel(object sender,
            System.Windows.Input.MouseWheelEventArgs e)
        {
            var viewModel = (ImageViewerViewModel)DataContext;
            viewModel.ChangeImage(-e.Delta);
        }
    }
}
