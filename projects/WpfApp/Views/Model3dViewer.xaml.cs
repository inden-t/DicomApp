using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using DicomApp.PresenterInterface;

namespace DicomApp.Views
{
    public partial class Model3dViewer : Window, IModel3dViewer
    {
        private Point _lastMousePosition;
        private bool _isRotating;
        private bool _isMiddleButtonDown;
        private PerspectiveCamera _camera;
        private Point3D _modelCenter;
        private double _cameraDistance;

        public Model3dViewer()
        {
            InitializeComponent();

            // ウィンドウのCloseイベントをハンドリング
            this.Closing += Model3dViewer_Closing;

            // カメラの初期化
            _camera = (PerspectiveCamera)viewport3D.Camera;
            _modelCenter = new Point3D(0, 0, 0);
            _cameraDistance = 700; // 初期カメラ距離
            UpdateCameraPosition();

            this.SizeChanged += (sender, e) => UpdateCenterMark();
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

        private void Model3dViewer_Closing(object sender,
            System.ComponentModel.CancelEventArgs e)
        {
            model3DGroup.Children.Clear(); // モデルをクリア
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _isRotating = true;
                _lastMousePosition = e.GetPosition(this);
                Mouse.Capture((IInputElement)sender);
            }
            else if (e.MiddleButton == MouseButtonState.Pressed)
            {
                _isMiddleButtonDown = true;
                _lastMousePosition = e.GetPosition(this);
                Mouse.Capture((IInputElement)sender);
            }
        }

        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
            {
                _isRotating = false;
            }

            if (e.MiddleButton == MouseButtonState.Released)
            {
                _isMiddleButtonDown = false;
            }

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
            else if (_isMiddleButtonDown)
            {
                Point currentPosition = e.GetPosition(this);
                Vector delta = currentPosition - _lastMousePosition;

                PanCamera(delta.X, delta.Y);

                _lastMousePosition = currentPosition;
            }
        }

        private void Grid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Shift)
            {
                MoveForwardBackward(e.Delta);
            }
            else
            {
                ZoomCamera(e.Delta);
            }
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
            // ズーム距離に基づいて感度を調整
            double panSpeed = 0.5 * ((_cameraDistance + 50) / 750);
            Vector3D right = Vector3D.CrossProduct(_camera.LookDirection,
                _camera.UpDirection);
            Vector3D up = _camera.UpDirection;

            Vector3D panVector = (right * -deltaX + up * deltaY) * panSpeed;
            _camera.Position += panVector;
            _modelCenter += panVector;

            UpdateCameraPosition();
        }

        private void ZoomCamera(double delta)
        {
            double zoomSpeed = 0.1;
            _cameraDistance -= delta * zoomSpeed;
            _cameraDistance =
                Math.Max(0, Math.Min(1000, _cameraDistance)); // カメラ距離の制限

            UpdateCameraPosition();
        }

        private void MoveForwardBackward(double delta)
        {
            double moveSpeed = 0.1;
            Vector3D moveDirection = _camera.LookDirection;
            moveDirection.Normalize();

            _camera.Position += moveDirection * delta * moveSpeed;
            _modelCenter += moveDirection * delta * moveSpeed;

            UpdateCameraPosition();
        }

        private void UpdateCameraPosition()
        {
            _camera.Position =
                _modelCenter - _camera.LookDirection * _cameraDistance /
                _camera.LookDirection.Length;
        }

        private void UpdateCenterMark()
        {
            Point center =
                new Point(CenterMarkCanvas.ActualWidth / 2,
                    CenterMarkCanvas.ActualHeight / 2);
            Canvas.SetLeft(CenterMark, center.X - CenterMark.Width / 2);
            Canvas.SetTop(CenterMark, center.Y - CenterMark.Height / 2);
        }
    }
}
