using System;
using System.Diagnostics;

namespace DicomApp.UseCases
{
    public interface IProgressWindow
    {
        void Start();
        void End();
        void SetWindowTitle(string text);
        void SetProgress(double progress);
        void SetStatusText(string text);
    }
}
