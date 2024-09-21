using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DicomApp.Views
{
    public partial class SelectionOverlayControl : UserControl
    {
        public static readonly DependencyProperty OverlaySourceProperty =
            DependencyProperty.Register("OverlaySource", typeof(ImageSource), typeof(SelectionOverlayControl));

        public static readonly DependencyProperty IsVisibleProperty =
            DependencyProperty.Register("IsVisible", typeof(bool), typeof(SelectionOverlayControl));

        public ImageSource OverlaySource
        {
            get { return (ImageSource)GetValue(OverlaySourceProperty); }
            set { SetValue(OverlaySourceProperty, value); }
        }

        public bool IsVisible
        {
            get { return (bool)GetValue(IsVisibleProperty); }
            set { SetValue(IsVisibleProperty, value); }
        }

        public SelectionOverlayControl()
        {
            InitializeComponent();
        }
    }
}
