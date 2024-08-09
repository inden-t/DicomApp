using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using DicomApp.UseCases;

namespace DicomApp.Views
{
    public partial class BloodVesselPointCloud3DViewer : Window,
        IBloodVesselPointCloud3DViewer
    {
        private Point _lastMousePosition;
        private bool _isRotating;
        private PerspectiveCamera _camera;

        public BloodVesselPointCloud3DViewer()
        {
            InitializeComponent();

            // ウィンドウのCloseイベントをハンドリング
            this.Closing += BloodVesselPointCloud3DViewer_Closing;

            // カメラの初期化
            _camera = (PerspectiveCamera)viewport3D.Camera;
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

        private void Viewport3D_MouseLeftButtonDown(object sender,
            MouseButtonEventArgs e)
        {
            _isRotating = true;
            _lastMousePosition = e.GetPosition(this);
        }

        private void Viewport3D_MouseLeftButtonUp(object sender,
            MouseButtonEventArgs e)
        {
            _isRotating = false;
        }

        private void Viewport3D_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isRotating)
            {
                Point currentPosition = e.GetPosition(this);
                Vector delta = currentPosition - _lastMousePosition;

                RotateCamera(delta.X, delta.Y);

                _lastMousePosition = currentPosition;
            }
        }

        private void Viewport3D_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            ZoomCamera(e.Delta);
        }

        private void RotateCamera(double deltaX, double deltaY)
        {
            double rotationSpeed = 0.5;
            Vector3D lookDirection = _camera.LookDirection;
            Vector3D upDirection = _camera.UpDirection;

            // Y軸周りの回転
            Matrix3D rotationY = new Matrix3D();
            rotationY.Rotate(new Quaternion(upDirection,
                -deltaX * rotationSpeed));

            // X軸周りの回転
            Vector3D rightDirection =
                Vector3D.CrossProduct(lookDirection, upDirection);
            Matrix3D rotationX = new Matrix3D();
            rotationX.Rotate(new Quaternion(rightDirection,
                deltaY * rotationSpeed));

            // 回転を適用
            _camera.LookDirection =
                rotationY.Transform(rotationX.Transform(lookDirection));
            _camera.UpDirection =
                rotationY.Transform(rotationX.Transform(upDirection));
        }

        private void ZoomCamera(double delta)
        {
            double zoomSpeed = 0.001;
            Vector3D lookDirection = _camera.LookDirection;
            _camera.Position += lookDirection * delta * zoomSpeed;
        }
    }
}
