using System;
using System.Reactive.Disposables;
using Prism.Mvvm;

public abstract class ViewModelBase : BindableBase, IDisposable
{
    public ViewModelBase()
    {
    }

    #region "IDisposable"

    public CompositeDisposable disposedValue = new CompositeDisposable();

    protected virtual void Dispose(bool disposing)
    {
        this.disposedValue.Dispose();
    }

    public void Dispose()
    {
        // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    #endregion
}
