using System.Windows;

namespace DicomApp.Views
{
    public partial class ProgressWindow : Window
    {
        public ProgressWindow()
        {
            InitializeComponent();
        }

        public double Progress
        {
            get { return (double)GetValue(ProgressProperty); }
            set { SetValue(ProgressProperty, value); }
        }

        public static readonly DependencyProperty ProgressProperty =
            DependencyProperty.Register("Progress", typeof(double),
                typeof(ProgressWindow), new PropertyMetadata(0.0));

        public string StatusText
        {
            get { return (string)GetValue(StatusTextProperty); }
            set { SetValue(StatusTextProperty, value); }
        }

        public static readonly DependencyProperty StatusTextProperty =
            DependencyProperty.Register("StatusText", typeof(string),
                typeof(ProgressWindow), new PropertyMetadata(""));
    }
}
