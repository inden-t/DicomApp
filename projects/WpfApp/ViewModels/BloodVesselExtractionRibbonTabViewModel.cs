using System;
using Reactive.Bindings;

namespace DicomApp.ViewModels
{
    public class BloodVesselExtractionRibbonTabViewModel : ViewModelBase
    {
        private readonly ImageViewerViewModel _imageViewerViewModel;

        public ReactiveCommand StartBloodVesselSelectionCommand { get; } =
            new();

        public ReactiveCommand EndBloodVesselSelectionCommand { get; } = new();
        public ReactiveCommand Execute3DFillSelectionCommand { get; } = new();
        public ReactiveCommand Clear2DFillSelectionCommand { get; } = new();
        public ReactiveCommand Clear3DFillSelectionCommand { get; }
        public ReactiveCommand BloodVesselExtractionCommand { get; } = new();
        public ReactiveCommand CancelExtractionCommand { get; } = new();
        public ReactiveCommand SaveModelCommand { get; } = new();

        public ReactiveProperty<double> ThresholdValue { get; } = new(128);

        public ReactiveProperty<bool> IsSelectionModeActive { get; } =
            new(false);

        public BloodVesselExtractionRibbonTabViewModel(
            ImageViewerViewModel imageViewerViewModel)
        {
            _imageViewerViewModel = imageViewerViewModel;

            StartBloodVesselSelectionCommand.Subscribe(() =>
                IsSelectionModeActive.Value = true);
            EndBloodVesselSelectionCommand.Subscribe(() =>
                IsSelectionModeActive.Value = false);

            Execute3DFillSelectionCommand.Subscribe(() =>
                _imageViewerViewModel.CurrentSelectionMode.Value =
                    SelectionMode.Fill3DSelection);
            Clear3DFillSelectionCommand = new ReactiveCommand()
                .WithSubscribe(() =>
                    _imageViewerViewModel.CurrentSelectionMode.Value =
                        SelectionMode.Clear3DFillSelection);
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

            BloodVesselExtractionCommand.Subscribe(
                ExecuteBloodVesselExtraction);
            CancelExtractionCommand.Subscribe(CancelBloodVesselExtraction);
            SaveModelCommand.Subscribe(SaveExtractedModel);
        }

        private void ExecuteBloodVesselExtraction()
        {
            // 血管抽出処理の実装
            Console.WriteLine("血管抽出処理を開始します。しきい値: " + ThresholdValue.Value);
        }

        private void CancelBloodVesselExtraction()
        {
            // 血管抽出処理のキャンセル実装
            Console.WriteLine("血管抽出処理をキャンセルしました。");
        }

        private void SaveExtractedModel()
        {
            // 抽出されたモデルの保存処理の実装
            Console.WriteLine("抽出されたモデルを保存します。");
        }
    }
}
