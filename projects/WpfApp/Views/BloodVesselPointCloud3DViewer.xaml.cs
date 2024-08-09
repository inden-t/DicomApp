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
        private bool _isPanning;
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

        private void Grid_MouseLeftButtonDown(object sender,
            MouseButtonEventArgs e)
        {
            _isRotating = true;
            _lastMousePosition = e.GetPosition(this);
            Mouse.Capture((IInputElement)sender);
        }

        private void Grid_MouseLeftButtonUp(object sender,
            MouseButtonEventArgs e)
        {
            _isRotating = false;
            Mouse.Capture(null);
        }

        private void Grid_MouseRightButtonDown(object sender,
            MouseButtonEventArgs e)
        {
            _isPanning = true;
            _lastMousePosition = e.GetPosition(this);
            Mouse.Capture((IInputElement)sender);
        }

        private void Grid_MouseRightButtonUp(object sender,
            MouseButtonEventArgs e)
        {
            _isPanning = false;
            Mouse.Capture(null);
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isRotating)
            {
                Point currentPosition = e.GetPosition(this);
                Vector delta = currentPosition - _lastMousePosition;

                RotateCamera(delta.X, delta.Y);

                _lastMousePosition = currentPosition;
            }
            else if (_isPanning)
            {
                Point currentPosition = e.GetPosition(this);
                Vector delta = currentPosition - _lastMousePosition;

                PanCamera(delta.X, delta.Y);

                _lastMousePosition = currentPosition;
            }
        }

        private void Grid_MouseWheel(object sender, MouseWheelEventArgs e)
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

            // Z軸周りの回転（マウスの横方向の動き）
            rotation.Rotate(new Quaternion(new Vector3D(0, 0, 1),
                deltaX * rotationSpeed));

            // 右方向ベクトル周りの回転（マウスの縦方向の動き）
            rotation.Rotate(new Quaternion(rightDirection,
                -deltaY * rotationSpeed));

            // 回転を適用
            _camera.LookDirection = rotation.Transform(lookDirection);
            _camera.UpDirection = rotation.Transform(upDirection);

            // カメラ位置を更新
            UpdateCameraPosition();
        }

        private void PanCamera(double deltaX, double deltaY)
        {
            double panSpeed = 0.5;
            Vector3D right = Vector3D.CrossProduct(_camera.LookDirection,
                _camera.UpDirection);
            Vector3D up = _camera.UpDirection;

            _camera.Position += (right * -deltaX + up * deltaY) * panSpeed;
            _modelCenter += (right * -deltaX + up * deltaY) * panSpeed;

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
