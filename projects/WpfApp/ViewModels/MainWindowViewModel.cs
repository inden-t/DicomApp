using System;
using System.Windows;
using Reactive.Bindings;
using System.Windows.Media.Imaging;
using DicomApp.Views;

namespace DicomApp.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private DICOMFile _dicomFile;
        private ImageViewerViewModel _imageViewerViewModel;

        public ReactiveProperty<BitmapSource> RenderedImage { get; }

        public ReactiveCommand OpenDICOMFileCommand { get; } = new();
        public ReactiveCommand ExitCommand { get; } = new();
        public ReactiveCommand ZoomInCommand { get; } = new();
        public ReactiveCommand ZoomOutCommand { get; } = new();
        public ReactiveCommand PanCommand { get; } = new();
        public ReactiveCommand RotateCommand { get; } = new();

        public MainWindowViewModel(
            ImageViewerViewModel imageViewerViewModel)
        {
            _imageViewerViewModel = imageViewerViewModel;

            RenderedImage = _imageViewerViewModel.BitmapSourceImage;

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
                    _imageViewerViewModel.SetImage(_dicomFile.GetImage());
                    RenderedImage.Value =
                        _imageViewerViewModel.BitmapSourceImage.Value;
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
            _imageViewerViewModel.Zoom(1.25);
        }

        public void ZoomOut()
        {
            _imageViewerViewModel.Zoom(0.8);
        }

        public void Pan(double x, double y)
        {
            _imageViewerViewModel.Pan(x, y);
        }

        public void Rotate(double angle)
        {
            _imageViewerViewModel.Rotate(angle);
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
