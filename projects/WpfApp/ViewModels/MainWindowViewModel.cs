using System;
using System.Windows;
using DicomApp.UseCases;
using Reactive.Bindings;

namespace DicomApp.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly ImageViewerViewModel _imageViewerViewModel;
        private readonly BloodVesselExtractionUseCase _extractionUseCase;

        public ReactiveCommand OpenDicomFileCommand { get; } = new();
        public ReactiveCommand OpenDicomFolderCommand { get; } = new();
        public ReactiveCommand ExitCommand { get; } = new();
        public ReactiveCommand ZoomInCommand { get; } = new();
        public ReactiveCommand ZoomOutCommand { get; } = new();

        public ReactiveCommand GeneratePointCloudCommand { get; } =
            new();

        public ReactiveCommand GenerateSurfaceModelCommand { get; } =
            new();

        public ReactiveCommand GenerateSurfaceModelLinearInterpolationCommand
        {
            get;
        } = new();

        public ReactiveCollection<DICOMFile> DicomFiles { get; } = new();

        public ReactiveProperty<int> SelectedIndex { get; } = new();

        public MainWindowViewModel(ImageViewerViewModel imageViewerViewModel)
        {
            _imageViewerViewModel = imageViewerViewModel;
            _extractionUseCase = new BloodVesselExtractionUseCase(
                new BloodVessel3DRegionSelector(),
                new BloodVesselSurfaceModelGenerator());

            ZoomInCommand.Subscribe(_ => ZoomIn());
            ZoomOutCommand.Subscribe(_ => ZoomOut());

            ExitCommand.Subscribe(_ => Application.Current.Shutdown());

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

        public async Task ExtractBloodVesselAsync()
        {
            //var progress = new Progress<int>(value => ProgressValue = value);
            var model =
                await _extractionUseCase.ExtractBloodVesselAsync(
                    //Threshold
                    //, progress
                );

            // 生成されたモデルを3Dビューワーに表示
            //_model3dViewer.SetModel(model.ModelGroup);
        }
    }
}
