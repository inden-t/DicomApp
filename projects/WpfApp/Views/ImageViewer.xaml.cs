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
    }
}
