using System.Windows;
using System.Windows.Media.Media3D;
using DicomApp.UseCases;

namespace DicomApp.Views
{
    public partial class BloodVessel3DViewer : Window, IBloodVessel3DViewer

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
