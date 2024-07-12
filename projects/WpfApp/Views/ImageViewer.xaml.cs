using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DicomApp.ViewModels;

namespace DicomApp.Views
{
    public partial class ImageViewer : UserControl
    {
        private readonly ImageViewerViewModel _viewModel;
        private Point _lastMousePosition;
        private bool _isScrolling;

        public ImageViewer(ImageViewerViewModel imageViewerViewModel)
        {
            InitializeComponent();

            _viewModel = imageViewerViewModel;
            DataContext = imageViewerViewModel;
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
        }

        private void ScrollViewer_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Released)
            {
                _isScrolling = false;
                ImageScrollViewer.Cursor = Cursors.Arrow;
                e.Handled = true;
            }
        }

        private void ScrollViewer_PreviewMouseWheel(object sender,
            MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                // Ctrlキーが押されている場合は、ズーム処理を行う
                double zoomFactor = e.Delta > 0 ? 1.1 : 0.9;
                _viewModel.Zoom(zoomFactor);
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
