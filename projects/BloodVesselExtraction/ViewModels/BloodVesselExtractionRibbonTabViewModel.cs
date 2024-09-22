using DicomApp.BloodVesselExtraction.Models;
using DicomApp.BloodVesselExtraction.UseCases;
using DicomApp.WpfUtilities.ViewModels;
using Reactive.Bindings;

namespace DicomApp.BloodVesselExtraction.ViewModels
{
    public class BloodVesselExtractionRibbonTabViewModel : ViewModelBase
    {
        private readonly SelectionOverlayControlViewModel
            _overlayControlViewModel;

        private readonly BloodVessel3DRegionSelector _regionSelector;

        public SelectionOverlayControlViewModel
            SelectionOverlayControlViewModel => _overlayControlViewModel;

        public ReactiveCommand Execute3DFillSelectionCommand { get; } = new();
        public ReactiveCommand Clear3DFillSelectionCommand { get; } = new();
        public ReactiveCommand Execute2DFillSelectionCommand { get; } = new();
        public ReactiveCommand Clear2DFillSelectionCommand { get; } = new();

        public ReactiveCommand UndoSelectionCommand { get; } = new();
        public ReactiveCommand RedoSelectionCommand { get; } = new();
        public ReactiveCommand SaveSelectionCommand { get; } = new();
        public ReactiveCommand LoadSelectionCommand { get; } = new();
        public ReactiveCommand ClearAllSelectionCommand { get; } = new();

        public ReactiveCommand BloodVesselExtractionCommand { get; } = new();

        public ReactiveCommand DiscardSelectionCommand { get; } = new();

        public ReactiveProperty<bool> CanUndo { get; } = new(false);
        public ReactiveProperty<bool> CanRedo { get; } = new(false);
        public ReactiveProperty<int> Threshold { get; } = new(220);

        public BloodVesselExtractionRibbonTabViewModel(
            SelectionOverlayControlViewModel overlayControlViewModel,
            BloodVessel3DRegionSelector regionSelector)
        {
            _overlayControlViewModel = overlayControlViewModel;
            _regionSelector = regionSelector;
            _regionSelector.UndoRedoStateChanged += (sender, e) =>
            {
                CanUndo.Value = e.CanUndo;
                CanRedo.Value = e.CanRedo;
            };
            _regionSelector.ThresholdChanged += (sender, threshold) =>
            {
                Threshold.Value = threshold;
            };

            Execute3DFillSelectionCommand.Subscribe(() =>
            {
                if (_overlayControlViewModel.CurrentSelectionMode.Value ==
                    SelectionMode.Fill3DSelection)
                {
                    _overlayControlViewModel.CurrentSelectionMode.Value =
                        SelectionMode.None;
                }
                else
                {
                    _overlayControlViewModel.CurrentSelectionMode.Value =
                        SelectionMode.Fill3DSelection;
                }
            });

            Clear3DFillSelectionCommand.Subscribe(() =>
            {
                if (_overlayControlViewModel.CurrentSelectionMode.Value ==
                    SelectionMode.Clear3DFillSelection)
                {
                    _overlayControlViewModel.CurrentSelectionMode.Value =
                        SelectionMode.None;
                }
                else
                {
                    _overlayControlViewModel.CurrentSelectionMode.Value =
                        SelectionMode.Clear3DFillSelection;
                }
            });

            Execute2DFillSelectionCommand.Subscribe(() =>
            {
                if (_overlayControlViewModel.CurrentSelectionMode.Value ==
                    SelectionMode.Fill2DSelection)
                {
                    _overlayControlViewModel.CurrentSelectionMode.Value =
                        SelectionMode.None;
                }
                else
                {
                    _overlayControlViewModel.CurrentSelectionMode.Value =
                        SelectionMode.Fill2DSelection;
                }
            });

            Clear2DFillSelectionCommand.Subscribe(() =>
            {
                if (_overlayControlViewModel.CurrentSelectionMode.Value ==
                    SelectionMode.ClearFill2DSelection)
                {
                    _overlayControlViewModel.CurrentSelectionMode.Value =
                        SelectionMode.None;
                }
                else
                {
                    _overlayControlViewModel.CurrentSelectionMode.Value =
                        SelectionMode.ClearFill2DSelection;
                }
            });
        }

        public void InitializeDependencies(
            BloodVesselExtractionUseCase bloodVesselExtractionUseCase,
            ManageBloodVesselRegionUseCase manageBloodVesselRegionUseCase)
        {
            BloodVesselExtractionCommand.Subscribe(async () =>
                await bloodVesselExtractionUseCase.ExtractBloodVesselAsync());

            UndoSelectionCommand.Subscribe(() =>
                manageBloodVesselRegionUseCase.UndoSelection());
            RedoSelectionCommand.Subscribe(() =>
                manageBloodVesselRegionUseCase.RedoSelection());
            SaveSelectionCommand.Subscribe(() =>
                manageBloodVesselRegionUseCase.SaveSelectedRegion());
            LoadSelectionCommand.Subscribe(() =>
                manageBloodVesselRegionUseCase.LoadSelectedRegion());
            ClearAllSelectionCommand.Subscribe(() =>
                manageBloodVesselRegionUseCase.ClearAllSelection());

            DiscardSelectionCommand.Subscribe(() =>
                manageBloodVesselRegionUseCase.EndRegionSelection());
        }
    }
}
