using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using DicomApp.BloodVesselExtraction.Views;
using DicomApp.WpfApp.ViewModels;

namespace DicomApp.WpfApp.Views
{
    public partial class ImageViewer : UserControl
    {
        private readonly ImageViewerViewModel _viewModel;
        private Point _lastMousePosition;
        private bool _isScrolling;

        public ImageViewer(ImageViewerViewModel imageViewerViewModel,
            SelectionOverlayControl selectionOverlay)
        {
            InitializeComponent();

            _viewModel = imageViewerViewModel;
            DataContext = _viewModel;

            SelectionOverlay.Content = selectionOverlay;
        }

        private void UserControl_MouseWheel(object sender,
            MouseWheelEventArgs e)
        {
            if (!e.Handled)
            {
                int offset = Math.Sign(-e.Delta);
                _viewModel.SwitchImageByOffset(offset);
                e.Handled = true;
            }
        }

        private void UserControl_SizeChanged(object sender,
            SizeChangedEventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.UpdateViewerSize(e.NewSize.Width, e.NewSize.Height);
            }
        }

        private void ScrollViewer_MouseDown(object sender,
            MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                _isScrolling = true;
                _lastMousePosition = e.GetPosition(ImageScrollViewer);
                ImageScrollViewer.Cursor = Cursors.SizeAll;
                e.Handled = true;

                // マウスキャプチャを設定
                Mouse.Capture((IInputElement)sender);
            }
        }

        private void ScrollViewer_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isScrolling)
            {
                Point currentPosition = e.GetPosition(ImageScrollViewer);
                double deltaX = currentPosition.X - _lastMousePosition.X;
                double deltaY = currentPosition.Y - _lastMousePosition.Y;

                ImageScrollViewer.ScrollToHorizontalOffset(
                    ImageScrollViewer.HorizontalOffset - deltaX);
                ImageScrollViewer.ScrollToVerticalOffset(
                    ImageScrollViewer.VerticalOffset - deltaY);

                _lastMousePosition = currentPosition;
                e.Handled = true;
            }
            else
            {
                Point mousePosition = e.GetPosition(DicomImage);
                double x = mousePosition.X / DicomImage.ActualWidth;
                double y = mousePosition.Y / DicomImage.ActualHeight;

                _viewModel.ImageScrollViewer_MouseMove(x, y);
            }
        }

        private void ScrollViewer_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Released)
            {
                if (_isScrolling)
                {
                    _isScrolling = false;
                    ImageScrollViewer.Cursor = Cursors.Arrow;
                    Mouse.Capture(null); // マウスキャプチャを解放
                }

                e.Handled = true;
            }
        }

        private void ScrollViewer_PreviewMouseWheel(object sender,
            MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                // Ctrlキーが押されている場合は、ズーム処理を行う
                double zoomFactor = e.Delta > 0 ? 1.1 : 1.0 / 1.1;

                // マウスの位置を取得
                Point mousePos = e.GetPosition(DicomImage);

                // ズーム前のスクロール位置を保存
                double horizontalOffset = ImageScrollViewer.HorizontalOffset;
                double verticalOffset = ImageScrollViewer.VerticalOffset;

                // ズーム処理
                bool isZoomed = _viewModel.SetZoomValue(zoomFactor);

                if (isZoomed)
                {
                    // スクロール位置を調整
                    ImageScrollViewer.ScrollToHorizontalOffset(
                        mousePos.X * zoomFactor - mousePos.X +
                        horizontalOffset);
                    ImageScrollViewer.ScrollToVerticalOffset(
                        mousePos.Y * zoomFactor - mousePos.Y + verticalOffset);
                }

                e.Handled = true;
            }
            else
            {
                // Ctrlキーが押されていない場合は、イベントを親コントロールに渡す
                if (!e.Handled)
                {
                    // ScrollViewerがマウスホイールイベントをキャプチャするのを防ぐ
                    e.Handled = true;

                    // e は Handled = true で終えるため、新しいArgsオブジェクトでイベントを発行する
                    var eventArg = new MouseWheelEventArgs(e.MouseDevice,
                        e.Timestamp, e.Delta);
                    eventArg.RoutedEvent = UIElement.MouseWheelEvent;
                    eventArg.Source = sender;
                    var parent = ((Control)sender).Parent as UIElement;
                    parent.RaiseEvent(eventArg);
                }
            }
        }
    }
}
