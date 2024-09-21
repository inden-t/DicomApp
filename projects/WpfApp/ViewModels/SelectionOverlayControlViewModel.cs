using System.Windows.Media;
using Reactive.Bindings;

namespace DicomApp.ViewModels
{
    public class SelectionOverlayControlViewModel : ViewModelBase
    {
        public ReactiveProperty<ImageSource> OverlaySource { get; }
        public ReactiveProperty<bool> IsVisible { get; }

        public SelectionOverlayControlViewModel()
        {
            OverlaySource = new ReactiveProperty<ImageSource>();
            IsVisible = new ReactiveProperty<bool>(true);
        }

        public void SetOverlaySource(ImageSource source)
        {
            OverlaySource.Value = source;
        }

        public void SetVisibility(bool isVisible)
        {
            IsVisible.Value = isVisible;
        }
    }
}
