using System;

namespace DicomApp.PresenterInterface
{
    public interface IProgressWindow
    {
        void Start();
        void End();
        void SetWindowTitle(string text);
        void SetStatusText(string text);
        void SetProgress(double progress);
    }
}
