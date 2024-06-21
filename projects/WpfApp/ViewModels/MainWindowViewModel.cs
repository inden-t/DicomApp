using System;
using System.Collections.ObjectModel;
using System.Windows;
using Reactive.Bindings;
using System.Windows.Media.Imaging;
using DicomApp.Views;
using Microsoft.Win32;

namespace DicomApp.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ObservableCollection<DICOMFile> _dicomFiles =
            new ObservableCollection<DICOMFile>();

        private ImageViewerViewModel _imageViewerViewModel;

        public ReactiveCommand OpenDICOMFileCommand { get; } = new();
        public ReactiveCommand ExitCommand { get; } = new();
        public ReactiveCommand ZoomInCommand { get; } = new();
        public ReactiveCommand ZoomOutCommand { get; } = new();
        public ReactiveCommand PanCommand { get; } = new();
        public ReactiveCommand RotateCommand { get; } = new();

        public ObservableCollection<DICOMFile> DicomFiles
        {
            get => _dicomFiles;
            set => SetProperty(ref _dicomFiles, value);
        }

        public ReactiveProperty<DICOMFile> SelectedDicomFile { get; } =
            new ReactiveProperty<DICOMFile>();

        private int _currentImageIndex = 0;

        public MainWindowViewModel(ImageViewerViewModel imageViewerViewModel)
        {
            _imageViewerViewModel = imageViewerViewModel;

            OpenDICOMFileCommand.Subscribe(_ => { OpenDICOMFile(); });
            //ExitCommand          .Subscribe(_ => { OpenDICOMFile(); });
            ZoomInCommand.Subscribe(_ => { ZoomIn(); });
            ZoomOutCommand.Subscribe(_ => { ZoomOut(); });
            //PanCommand           .Subscribe(_ => { Pan(); });
            //RotateCommand        .Subscribe(_ => { Rotate(); });

            _imageViewerViewModel.ChangeImageCommand.Subscribe(delta =>
                ChangeImage(delta));

            SelectedDicomFile.Subscribe(file =>
            {
                if (file != null)
                {
                    _currentImageIndex = DicomFiles.IndexOf(file);
                    UpdateDisplayedImage();
                }
            });
        }

        private void ChangeImage(int delta)
        {
            if (DicomFiles.Count == 0) return;

            _currentImageIndex += delta > 0 ? 1 : -1;
            _currentImageIndex = Math.Max(0,
                Math.Min(_currentImageIndex, DicomFiles.Count - 1));

            UpdateDisplayedImage();
        }

        private void UpdateDisplayedImage()
        {
            var selectedFile = DicomFiles[_currentImageIndex];
            _imageViewerViewModel.SetImage(selectedFile.GetImage());
            SelectedDicomFile.Value = selectedFile;
        }

        public void OpenDICOMFile()
        {
            string[] filePaths = GetFilePaths();

            if (filePaths != null && filePaths.Length > 0)
            {
                DicomFiles.Clear();

                foreach (string filePath in filePaths)
                {
                    try
                    {
                        DICOMFile dicomFile = new DICOMFile(filePath);
                        dicomFile.Load();
                        DicomFiles.Add(dicomFile);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            $"Error opening DICOM file: {ex.Message}",
                            "Error", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }

                if (DicomFiles.Count > 0)
                {
                    _imageViewerViewModel.SetImage(DicomFiles[0].GetImage());
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

        private string[] GetFilePaths()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter =
                "DICOM ファイル (*.dcm)|*.dcm|すべてのファイル (*.*)|*.*";
            openFileDialog.Title = "DICOM ファイルを選択してください";
            openFileDialog.Multiselect = true;

            if (openFileDialog.ShowDialog() == true)
            {
                return openFileDialog.FileNames;
            }

            return null;
        }
    }
}
