using System.Windows;
using DicomApp.BloodVesselExtraction.UseCases;
using DicomApp.BloodVesselExtraction.ViewModels;
using DicomApp.MainUseCases.UseCases;
using DicomApp.WpfApp.UseCases;
using DicomApp.WpfUtilities.ViewModels;
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

        public ImageViewerViewModel ImageViewerViewModel =>
            _imageViewerViewModel;

        public SelectionOverlayControlViewModel
            SelectionOverlayControlViewModel => _overlayControlViewModel;

        public ReactiveCommand OpenDicomFileCommand { get; } = new();
        public ReactiveCommand OpenDicomFolderCommand { get; } = new();
        public ReactiveCommand LoadModelCommand { get; } = new();
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

        public ReactiveProperty<int> SelectedRibbonTabIndex { get; } = new();

        public ReactiveProperty<int> ThresholdValue { get; } = new(220);
        public ReactiveProperty<int> ThresholdUpperLimit { get; } = new(255);

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

            StartBloodVesselSelectionCommand.Subscribe(() =>
            {
                _overlayControlViewModel.IsSelectionModeActive.Value = true;
                SelectedRibbonTabIndex.Value = 1; // 血管抽出タブ
                _select3DBloodVesselRegionUseCase?.StartSelection(ThresholdValue
                    .Value, ThresholdUpperLimit.Value);
                _bloodVesselExtractionUseCase?.SetThreshold(ThresholdValue
                    .Value, ThresholdUpperLimit.Value);
            });

            _overlayControlViewModel.IsSelectionModeActive.Subscribe((value) =>
                IsBloodVesselExtractionUiEnabled.Value = !value);
        }


        public void InitializeDependencies(
            OpenDicomFileUseCase openDicomFileUseCase,
            LoadModel3dUseCase loadModel3dUseCase,
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
            LoadModelCommand.Subscribe(async _ =>
                await loadModel3dUseCase.ExecuteAsync());

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

        public void ZoomIn()
        {
            _imageViewerViewModel.SetZoomValue(1.25);
        }

        public void ZoomOut()
        {
            _imageViewerViewModel.SetZoomValue(0.8);
        }
    }
}
