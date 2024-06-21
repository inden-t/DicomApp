// cs
// 以下のように Image Viewer クラスを実装することができます。
// 
// ```csharp

using System;
using System.Windows.Controls;
using DICOMViewer.ViewModels;

namespace DICOMViewer.Views
{
    public partial class ImageViewer : UserControl
    {
        public ImageViewer(ImageViewerViewModel imageViewerViewModel)
        {
            InitializeComponent();

            DataContext = imageViewerViewModel;
        }
    }
}
// ```
// 
// このクラスでは、DICOM画像の表示と操作を行うことができます。
// 
// - `SetImage(DicomImage image)`: DICOM画像を設定します。
// - `Zoom(double factor)`: 画像をズームします。
// - `Pan(double x, double y)`: 画像をパンします。
// - `Rotate(double angle)`: 画像を回転します。
// - `Render()`: 画像を描画します。
// 
// このクラスは、ユーザー操作に応じて適切なメソッドを呼び出すことで、DICOM画像の表示と操作を行うことができます。
