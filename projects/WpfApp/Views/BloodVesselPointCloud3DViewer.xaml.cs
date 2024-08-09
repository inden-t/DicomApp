using System.Windows;
using System.Windows.Media.Media3D;
using DicomApp.UseCases;

namespace DicomApp.Views
{
    public partial class BloodVesselPointCloud3DViewer : Window,
        IBloodVesselPointCloud3DViewer
    {
        public BloodVesselPointCloud3DViewer()
        {
            InitializeComponent();

            // ウィンドウのCloseイベントをハンドリング
            this.Closing += BloodVesselPointCloud3DViewer_Closing;
        }

        public void SetModel(Model3DGroup model)
        {
            model3DGroup.Children.Clear();
            model3DGroup.Children.Add(model);
        }

        private void BloodVesselPointCloud3DViewer_Closing(object sender,
            System.ComponentModel.CancelEventArgs e)
        {
            model3DGroup.Children.Clear(); // モデルをクリア
        }
    }
}
