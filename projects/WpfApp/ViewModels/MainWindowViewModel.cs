using System;
using System.Collections.ObjectModel;
using Reactive.Bindings;

namespace DicomApp.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ImageViewerViewModel _imageViewerViewModel;
        private FileManager _fileManager;

        public ReactiveCommand OpenDICOMFileCommand { get; } = new();
        public ReactiveCommand ExitCommand { get; } = new();
        public ReactiveCommand ZoomInCommand { get; } = new();
        public ReactiveCommand ZoomOutCommand { get; } = new();
        public ReactiveCommand PanCommand { get; } = new();
        public ReactiveCommand RotateCommand { get; } = new();

        public ObservableCollection<DICOMFile> DicomFiles => _fileManager.DicomFiles;
        public ReactiveProperty<DICOMFile> SelectedDicomFile => _fileManager.SelectedDicomFile;

        public MainWindowViewModel(ImageViewerViewModel imageViewerViewModel,
            FileManager fileManager)
        {
            _imageViewerViewModel = imageViewerViewModel;
            _fileManager = fileManager;

            OpenDICOMFileCommand.Subscribe(_ =>
            {
                _fileManager.OpenDICOMFile();
            });
            ZoomInCommand.Subscribe(_ => { ZoomIn(); });
            ZoomOutCommand.Subscribe(_ => { ZoomOut(); });

            _imageViewerViewModel.ChangeImageCommand.Subscribe(delta =>
                _fileManager.ChangeImage(delta));

            SelectedDicomFile.Subscribe(file => UpdateDisplayedImage(file));
        }

        private void UpdateDisplayedImage(DICOMFile file)
        {
            if (file != null)
            {
                _imageViewerViewModel.SetImage(file.GetImage());
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
    }
}
