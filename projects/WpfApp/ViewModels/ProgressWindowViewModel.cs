using System;
using Reactive.Bindings;

namespace DicomApp.ViewModels
{
    public class ProgressWindowViewModel : ViewModelBase
    {
        public ReactiveProperty<string> WindowTitle { get; } = new();
        public ReactiveProperty<string> StatusText { get; } = new();
        public ReactiveProperty<double> Progress { get; } = new();
    }
}
