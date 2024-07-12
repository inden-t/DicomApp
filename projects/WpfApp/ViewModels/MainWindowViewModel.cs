using System;
using Reactive.Bindings;

namespace DicomApp.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly ImageViewerViewModel _imageViewerViewModel;

        public ReactiveCommand OpenDicomFileCommand { get; } = new();
        public ReactiveCommand OpenDicomFolderCommand { get; } = new();
        public ReactiveCommand ExitCommand { get; } = new();
        public ReactiveCommand ZoomInCommand { get; } = new();
        public ReactiveCommand ZoomOutCommand { get; } = new();
        public ReactiveCommand PanCommand { get; } = new();
        public ReactiveCommand RotateCommand { get; } = new();

        public ReactiveCollection<DICOMFile> DicomFiles { get; } = new();

        public ReactiveProperty<int> SelectedIndex { get; } = new();

        public MainWindowViewModel(ImageViewerViewModel imageViewerViewModel)
        {
            _imageViewerViewModel = imageViewerViewModel;

            ZoomInCommand.Subscribe(_ => ZoomIn());
            ZoomOutCommand.Subscribe(_ => ZoomOut());

            _imageViewerViewModel.SwitchImageByIndexCommand.Subscribe(index =>
                SwitchImageByIndex(index));

            _imageViewerViewModel.SwitchImageByOffsetCommand.Subscribe(offset =>
                SwitchImageByOffset(offset));

            SelectedIndex.Subscribe(index => ChangeDisplayedImage(index));

            // DicomFilesの値が変更されたときにMaximumScrollValueを更新する
            DicomFiles.CollectionChanged += (sender, e) =>
            {
                _imageViewerViewModel.SetMaximumScrollValue(
                    DicomFiles.Count - 1);
            };
        }

        private void SwitchImageByIndex(int index)
        {
            if (index >= 0 && index < DicomFiles.Count)
            {
                SelectedIndex.Value = index;
            }
        }

        private void SwitchImageByOffset(int offset)
        {
            if (DicomFiles.Count == 0) return;

            int newIndex = SelectedIndex.Value + offset;
            newIndex = Math.Max(0,
                Math.Min(newIndex, DicomFiles.Count - 1));
            SelectedIndex.Value = newIndex;
        }

        private void ChangeDisplayedImage(int index)
        {
            if (index < 0 || index >= DicomFiles.Count)
            {
                return;
            }

            var selectedFile = DicomFiles[index];
            if (selectedFile != null)
            {
                var image = selectedFile.GetImage();
                _imageViewerViewModel.SetImage(image);
                _imageViewerViewModel.ScrollValue.Value = index;
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
