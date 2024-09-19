using System;
using Reactive.Bindings;

namespace DicomApp.ViewModels
{
    public class BloodVesselExtractionRibbonTabViewModel : ViewModelBase
    {
        public ImageViewerViewModel ImageViewerViewModel =>
            _imageViewerViewModel;

        private readonly ImageViewerViewModel _imageViewerViewModel;

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

        public ReactiveProperty<double> ThresholdValue { get; } = new(128);

        public BloodVesselExtractionRibbonTabViewModel(
            ImageViewerViewModel imageViewerViewModel)
        {
            _imageViewerViewModel = imageViewerViewModel;

            DiscardSelectionCommand.Subscribe(() =>
                _imageViewerViewModel.IsSelectionModeActive.Value = false);

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
        }
    }
}
