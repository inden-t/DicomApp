using System;
using Reactive.Bindings;

namespace DicomApp.Models
{
    public class FileManager
    {
        public ReactiveCollection<DICOMFile> DicomFiles { get; } =
            new ReactiveCollection<DICOMFile>();

        public ReactiveProperty<DICOMFile> SelectedDicomFile { get; } =
            new ReactiveProperty<DICOMFile>();

        private int _currentImageIndex = 0;

        public void ClearFiles()
        {
            DicomFiles.Clear();
        }

        public void AddFile(DICOMFile file)
        {
            DicomFiles.Add(file);
        }

        public void SetSelectedFile(DICOMFile file)
        {
            SelectedDicomFile.Value = file;
        }

        public void SwitchImageByOffset(int delta)
        {
            if (DicomFiles.Count == 0) return;

            _currentImageIndex += delta > 0 ? 1 : -1;
            _currentImageIndex = Math.Max(0,
                Math.Min(_currentImageIndex, DicomFiles.Count - 1));

            UpdateSelectedDicomFile();
        }

        private void UpdateSelectedDicomFile()
        {
            SelectedDicomFile.Value = DicomFiles[_currentImageIndex];
        }
    }
}
