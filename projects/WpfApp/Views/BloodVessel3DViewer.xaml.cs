using System.Windows;
using System.Windows.Media.Media3D;

namespace DicomApp.Views
{
    public partial class BloodVessel3DViewer : Window
    {
        public BloodVessel3DViewer()
        {
            InitializeComponent();
        }

        public void SetModel(Model3DGroup model)
        {
            model3DGroup.Children.Clear();
            model3DGroup.Children.Add(model);
        }
    }
}
