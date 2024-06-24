using System;
using Reactive.Bindings;

namespace DicomApp.Models
{
    public class FileManager
    {
        public ReactiveCollection<DICOMFile> DicomFiles { get; } =
            new ReactiveCollection<DICOMFile>();

        public ReactiveProperty<int> SelectedIndex { get; } =
            new ReactiveProperty<int>(-1);

        public void ClearFiles()
        {
            DicomFiles.Clear();
            SelectedIndex.Value = -1;
        }

        public void AddFile(DICOMFile file)
        {
            DicomFiles.Add(file);
            if (SelectedIndex.Value == -1)
            {
                SelectedIndex.Value = 0;
            }
        }

        public void SetSelectedIndex(int index)
        {
            if (index >= 0 && index < DicomFiles.Count)
            {
                SelectedIndex.Value = index;
            }
        }

        public void SwitchImageByOffset(int delta)
        {
            if (DicomFiles.Count == 0) return;

            int newIndex = SelectedIndex.Value + (delta > 0 ? 1 : -1);
            newIndex = Math.Max(0, Math.Min(newIndex, DicomFiles.Count - 1));
            SelectedIndex.Value = newIndex;
        }

        public DICOMFile GetSelectedFile()
        {
            if (SelectedIndex.Value >= 0 &&
                SelectedIndex.Value < DicomFiles.Count)
            {
                return DicomFiles[SelectedIndex.Value];
            }

            return null;
        }
    }
}
