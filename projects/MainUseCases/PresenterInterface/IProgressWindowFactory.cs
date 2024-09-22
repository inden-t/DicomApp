using System;

namespace DicomApp.PresenterInterface
{
    public interface IProgressWindowFactory
    {
        IProgressWindow Create();
    }
}
