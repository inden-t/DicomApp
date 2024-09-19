using System;
using System.Windows;
using DicomApp.Models;
using Reactive.Bindings;

namespace DicomApp.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly ImageViewerViewModel _imageViewerViewModel;
        private readonly BloodVessel3DRegionSelector _regionSelector;

        public ImageViewerViewModel ImageViewerViewModel =>
            _imageViewerViewModel;

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

        public ReactiveCommand StartBloodVesselSelectionCommand { get; } =
            new();

        public ReactiveCollection<DICOMFile> DicomFiles { get; } = new();

        public ReactiveProperty<int> SelectedIndex { get; } = new();
        public ReactiveProperty<int> SelectedRibbonTabIndex { get; } = new();

        public MainWindowViewModel(ImageViewerViewModel imageViewerViewModel,
            BloodVessel3DRegionSelector regionSelector)
        {
            _imageViewerViewModel = imageViewerViewModel;
            _regionSelector = regionSelector;

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

            StartBloodVesselSelectionCommand.Subscribe(() =>
            {
                _imageViewerViewModel.IsSelectionModeActive.Value = true;
                SelectedRibbonTabIndex.Value = 1; // 血管抽出タブ
                _regionSelector.PreRenderImages();
            });
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
                _imageViewerViewModel.ScrollValue.Value = index;
                _imageViewerViewModel.SetImage(image);
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
            //var model =
            //    await _extractionUseCase.ExtractBloodVesselAsync(
            //        //Threshold
            //        //, progress
            //    );

            // 生成されたモデルを3Dビューワーに表示
            //_model3dViewer.SetModel(model.ModelGroup);
        }
    }
}
