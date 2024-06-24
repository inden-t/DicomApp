using System;
using Reactive.Bindings;
using DicomApp.Models;
using DicomApp.UseCases;

namespace DicomApp.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly ImageViewerViewModel _imageViewerViewModel;
        private readonly FileManager _fileManager;
        private readonly OpenDicomFileUseCase _openDicomFileUseCase;

        public ReactiveCommand OpenDICOMFileCommand { get; } = new();
        public ReactiveCommand ExitCommand { get; } = new();
        public ReactiveCommand ZoomInCommand { get; } = new();
        public ReactiveCommand ZoomOutCommand { get; } = new();
        public ReactiveCommand PanCommand { get; } = new();
        public ReactiveCommand RotateCommand { get; } = new();

        public ReactiveCollection<DICOMFile> DicomFiles =>
            _fileManager.DicomFiles;

        public ReactiveProperty<DICOMFile> SelectedDicomFile =>
            _fileManager.SelectedDicomFile;

        public MainWindowViewModel(ImageViewerViewModel imageViewerViewModel,
            FileManager fileManager, OpenDicomFileUseCase openDicomFileUseCase)
        {
            _imageViewerViewModel = imageViewerViewModel;
            _fileManager = fileManager;
            _openDicomFileUseCase = openDicomFileUseCase;

            OpenDICOMFileCommand.Subscribe(_ =>
                _openDicomFileUseCase.Execute());
            ZoomInCommand.Subscribe(_ => ZoomIn());
            ZoomOutCommand.Subscribe(_ => ZoomOut());

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
