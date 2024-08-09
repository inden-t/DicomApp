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
        private Point3D _modelCenter;
        private double _cameraDistance;

        public BloodVesselPointCloud3DViewer()
        {
            InitializeComponent();

            // ウィンドウのCloseイベントをハンドリング
            this.Closing += BloodVesselPointCloud3DViewer_Closing;

            // カメラの初期化
            _camera = (PerspectiveCamera)viewport3D.Camera;
            _modelCenter = new Point3D(0, 0, 0);
            _cameraDistance = 700; // 初期カメラ距離
            UpdateCameraPosition();
        }

        public void SetModel(Model3DGroup model)
        {
            model3DGroup.Children.Clear();
            model3DGroup.Children.Add(model);

            // モデルの中心を計算
            Rect3D bounds = model.Bounds;
            _modelCenter = new Point3D(
                (bounds.X + bounds.SizeX / 2),
                (bounds.Y + bounds.SizeY / 2),
                (bounds.Z + bounds.SizeZ / 2));

            // カメラ位置を更新
            UpdateCameraPosition();
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
            viewport3D.CaptureMouse();
        }

        private void Viewport3D_MouseLeftButtonUp(object sender,
            MouseButtonEventArgs e)
        {
            _isRotating = false;
            viewport3D.ReleaseMouseCapture();
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
            Vector3D rightDirection =
                Vector3D.CrossProduct(lookDirection, upDirection);

            // 現在の表示方向を基準にした回転
            Matrix3D rotation = Matrix3D.Identity;
            rotation.Rotate(
                new Quaternion(upDirection, -deltaX * rotationSpeed));
            rotation.Rotate(new Quaternion(rightDirection,
                -deltaY * rotationSpeed));

            // 回転を適用
            _camera.LookDirection = rotation.Transform(lookDirection);
            _camera.UpDirection = rotation.Transform(upDirection);

            // カメラ位置を更新
            UpdateCameraPosition();
        }

        private void ZoomCamera(double delta)
        {
            double zoomSpeed = 0.1;
            _cameraDistance -= delta * zoomSpeed;
            _cameraDistance =
                Math.Max(10, Math.Min(1000, _cameraDistance)); // カメラ距離の制限

            UpdateCameraPosition();
        }

        private void UpdateCameraPosition()
        {
            _camera.Position =
                _modelCenter - _camera.LookDirection * _cameraDistance /
                _camera.LookDirection.Length;
        }
    }
}
