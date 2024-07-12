using System;
using Reactive.Bindings;

namespace DicomApp.ViewModels
{
    public class ProgressWindowViewModel : ViewModelBase
    {
        public ReactiveProperty<double> Progress { get; } = new();
        public ReactiveProperty<string> StatusText { get; } = new();
    }
}
