using System;
using System.Windows.Controls;
using DICOMViewer.ViewModels;

namespace DICOMViewer.Views
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
