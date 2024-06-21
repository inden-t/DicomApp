// cs
// 要件定義書に基づいて、MainWindowViewModel クラスの実装を行います。このクラスは、メインウィンドウのビジネスロジックを担当します。
// 
// ```csharp

using System;
using System.Windows;
using Reactive.Bindings;
using System.Windows.Media.Imaging;

namespace DICOMViewer.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private DICOMFile _dicomFile;
        private ImageViewer _imageViewer;

        public ReactiveProperty<BitmapSource> RenderedImage { get; }

        public ReactiveCommand OpenDICOMFileCommand { get; } = new();
        public ReactiveCommand ExitCommand { get; } = new();
        public ReactiveCommand ZoomInCommand { get; } = new();
        public ReactiveCommand ZoomOutCommand { get; } = new();
        public ReactiveCommand PanCommand { get; } = new();
        public ReactiveCommand RotateCommand { get; } = new();

        public MainWindowViewModel(ImageViewer imageViewer)
        {
            _imageViewer = imageViewer;
            RenderedImage = _imageViewer.BitmapSourceImage;

            OpenDICOMFileCommand.Subscribe(_ => { OpenDICOMFile(); });
            //ExitCommand          .Subscribe(_ => { OpenDICOMFile(); });
            ZoomInCommand.Subscribe(_ => { ZoomIn(); });
            ZoomOutCommand.Subscribe(_ => { ZoomOut(); });
            //PanCommand           .Subscribe(_ => { Pan(); });
            //RotateCommand        .Subscribe(_ => { Rotate(); });
        }

        public void OpenDICOMFile()
        {
            // ファイルダイアログを表示し、ユーザーが選択したDICOMファイルのパスを取得する
            string filePath = GetFilePath();

            if (!string.IsNullOrEmpty(filePath))
            {
                try
                {
                    // DICOMファイルを読み込む
                    _dicomFile = new DICOMFile(filePath);
                    _dicomFile.Load();

                    // DICOMデータセットからイメージを取得し、ImageViewerに設定する
                    _imageViewer.SetImage(_dicomFile.GetImage());
                    RenderedImage.Value = _imageViewer.BitmapSourceImage.Value;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error opening DICOM file: {ex.Message}",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void ZoomIn()
        {
            _imageViewer.Zoom(1.25);
        }

        public void ZoomOut()
        {
            _imageViewer.Zoom(0.8);
        }

        public void Pan(double x, double y)
        {
            _imageViewer.Pan(x, y);
        }

        public void Rotate(double angle)
        {
            _imageViewer.Rotate(angle);
        }

        private string GetFilePath()
        {
            // ファイルダイアログを表示し、ユーザーが選択したファイルのパスを取得する
            // 実装は省略
            return
                @"C:\develop\DicomApp\.dcm\series-000001\image-000050.dcm";
            return
                "C:\\develop\\DicomApp\\.dcm\\Nodule154images\\JPCLN050.dcm";
        }
    }
}
// ```
// 
// このクラスは、以下の機能を提供します:
// 
// 1. **DICOM画像ファイルを開く**:
//    - `OpenDICOMFile()` メソッドを呼び出すと、ファイルダイアログを表示し、ユーザーが選択したDICOMファイルを読み込みます。
//    - DICOM画像データを取得し、`ImageViewer`に設定します。
// 
// 2. **DICOM画像をズームする**:
//    - `ZoomIn()` メソッドを呼び出すと、画像をズームインします。
//    - `ZoomOut()` メソッドを呼び出すと、画像をズームアウトします。
//    - `ImageViewer`の`Zoom()`メソッドを呼び出して、実際の拡大/縮小処理を行います。
// 
// 3. **DICOM画像をパンする**:
//    - `Pan()` メソッドを呼び出すと、画像をパンします。
//    - `ImageViewer`の`Pan()`メソッドを呼び出して、実際の画像移動処理を行います。
// 
// 4. **DICOM画像を回転する**:
//    - `Rotate()` メソッドを呼び出すと、画像を回転します。
//    - `ImageViewer`の`Rotate()`メソッドを呼び出して、実際の画像回転処理を行います。
// 
// このクラスは、ユーザーインターフェースからの操作を受け取り、適切
