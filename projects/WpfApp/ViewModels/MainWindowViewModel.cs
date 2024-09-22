using System.Windows;
using DicomApp.BloodVesselExtraction.UseCases;
using DicomApp.BloodVesselExtraction.ViewModels;
using DicomApp.CoreModels.Models;
using DicomApp.MainUseCases.UseCases;
using DicomApp.WpfApp.UseCases;
using Reactive.Bindings;

namespace DicomApp.WpfApp.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly ImageViewerViewModel _imageViewerViewModel;

        private readonly SelectionOverlayControlViewModel
            _overlayControlViewModel;

        private BloodVesselExtractionUseCase _bloodVesselExtractionUseCase;

        private Select3DBloodVesselRegionUseCase
            _select3DBloodVesselRegionUseCase;

        public SelectionOverlayControlViewModel
            SelectionOverlayControlViewModel => _overlayControlViewModel;

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

        public ReactiveProperty<int> ThresholdValue { get; } = new(220);

        public ReactiveProperty<bool>
            IsBloodVesselExtractionUiEnabled { get; } = new(true);

        public MainWindowViewModel(ImageViewerViewModel imageViewerViewModel,
            SelectionOverlayControlViewModel overlayControlViewModel)
        {
            _imageViewerViewModel = imageViewerViewModel;
            _overlayControlViewModel = overlayControlViewModel;

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
                _overlayControlViewModel.IsSelectionModeActive.Value = true;
                SelectedRibbonTabIndex.Value = 1; // 血管抽出タブ
                _select3DBloodVesselRegionUseCase?.StartSelection(ThresholdValue
                    .Value);
                _bloodVesselExtractionUseCase?.SetThreshold(ThresholdValue
                    .Value);
            });

            _overlayControlViewModel.IsSelectionModeActive.Subscribe((value) =>
                IsBloodVesselExtractionUiEnabled.Value = !value);
        }


        public void InitializeDependencies(
            OpenDicomFileUseCase openDicomFileUseCase,
            GeneratePointCloudUseCase generatePointCloudUseCase,
            GenerateSurfaceModelUseCase generateSurfaceModelUseCase,
            GenerateSurfaceModelLinearInterpolationUseCase
                generateSurfaceModelLinearInterpolationUseCase,
            BloodVesselExtractionUseCase bloodVesselExtractionUseCase,
            Select3DBloodVesselRegionUseCase select3DBloodVesselRegionUseCase)
        {
            OpenDicomFileCommand.Subscribe(async _ =>
                await openDicomFileUseCase.ExecuteAsync());
            OpenDicomFolderCommand.Subscribe(async _ =>
                await openDicomFileUseCase.ExecuteFolderAsync());

            GeneratePointCloudCommand.Subscribe(
                async () =>
                    await generatePointCloudUseCase.ExecuteAsync());

            GenerateSurfaceModelCommand.Subscribe(
                async () =>
                    await generateSurfaceModelUseCase.ExecuteAsync());

            GenerateSurfaceModelLinearInterpolationCommand
                .Subscribe(async () =>
                    await generateSurfaceModelLinearInterpolationUseCase
                        .ExecuteAsync());

            _bloodVesselExtractionUseCase = bloodVesselExtractionUseCase;
            _select3DBloodVesselRegionUseCase =
                select3DBloodVesselRegionUseCase;
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
    }
}
