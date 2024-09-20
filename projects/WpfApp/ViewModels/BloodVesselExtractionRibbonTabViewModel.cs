using System;
using DicomApp.Models;
using DicomApp.UseCases;
using Reactive.Bindings;

namespace DicomApp.ViewModels
{
    public class BloodVesselExtractionRibbonTabViewModel : ViewModelBase
    {
        public ImageViewerViewModel ImageViewerViewModel =>
            _imageViewerViewModel;

        private readonly ImageViewerViewModel _imageViewerViewModel;
        private readonly BloodVessel3DRegionSelector _regionSelector;

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
        public ReactiveProperty<bool> IsSelectionHidden { get; } = new(false);

        public BloodVesselExtractionRibbonTabViewModel(
            ImageViewerViewModel imageViewerViewModel,
            BloodVessel3DRegionSelector regionSelector)
        {
            _imageViewerViewModel = imageViewerViewModel;
            _regionSelector = regionSelector;
            _regionSelector.UndoRedoStateChanged += (sender, e) =>
            {
                CanUndo.Value = e.CanUndo;
                CanRedo.Value = e.CanRedo;
            };

            Execute3DFillSelectionCommand.Subscribe(() =>
            {
                if (_imageViewerViewModel.CurrentSelectionMode.Value ==
                    SelectionMode.Fill3DSelection)
                {
                    _imageViewerViewModel.CurrentSelectionMode.Value =
                        SelectionMode.None;
                }
                else
                {
                    _imageViewerViewModel.CurrentSelectionMode.Value =
                        SelectionMode.Fill3DSelection;
                }
            });

            Clear3DFillSelectionCommand.Subscribe(() =>
            {
                if (_imageViewerViewModel.CurrentSelectionMode.Value ==
                    SelectionMode.Clear3DFillSelection)
                {
                    _imageViewerViewModel.CurrentSelectionMode.Value =
                        SelectionMode.None;
                }
                else
                {
                    _imageViewerViewModel.CurrentSelectionMode.Value =
                        SelectionMode.Clear3DFillSelection;
                }
            });

            Execute2DFillSelectionCommand.Subscribe(() =>
            {
                if (_imageViewerViewModel.CurrentSelectionMode.Value ==
                    SelectionMode.Fill2DSelection)
                {
                    _imageViewerViewModel.CurrentSelectionMode.Value =
                        SelectionMode.None;
                }
                else
                {
                    _imageViewerViewModel.CurrentSelectionMode.Value =
                        SelectionMode.Fill2DSelection;
                }
            });

            Clear2DFillSelectionCommand.Subscribe(() =>
            {
                if (_imageViewerViewModel.CurrentSelectionMode.Value ==
                    SelectionMode.ClearFill2DSelection)
                {
                    _imageViewerViewModel.CurrentSelectionMode.Value =
                        SelectionMode.None;
                }
                else
                {
                    _imageViewerViewModel.CurrentSelectionMode.Value =
                        SelectionMode.ClearFill2DSelection;
                }
            });

            IsSelectionHidden.Subscribe(value =>
                ImageViewerViewModel.IsSelectionVisible.Value = !value);
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
                manageBloodVesselRegionUseCase.InitializeRegionSelection());
        }
    }
}
