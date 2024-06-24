using System;
using System.Collections.ObjectModel;
using System.Windows;
using Microsoft.Win32;
using Reactive.Bindings;

namespace DicomApp.ViewModels
{
    public class FileManager : ViewModelBase
    {
        private ObservableCollection<DICOMFile> _dicomFiles =
            new ObservableCollection<DICOMFile>();

        public ObservableCollection<DICOMFile> DicomFiles
        {
            get => _dicomFiles;
            set => SetProperty(ref _dicomFiles, value);
        }

        public ReactiveProperty<DICOMFile> SelectedDicomFile { get; } =
            new ReactiveProperty<DICOMFile>();

        private int _currentImageIndex = 0;

        public void OpenDICOMFile()
        {
            string[] filePaths = GetFilePaths();

            if (filePaths != null && filePaths.Length > 0)
            {
                DicomFiles.Clear();

                foreach (string filePath in filePaths)
                {
                    try
                    {
                        DICOMFile dicomFile = new DICOMFile(filePath);
                        dicomFile.Load();
                        DicomFiles.Add(dicomFile);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            $"Error opening DICOM file: {ex.Message}", "Error",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                if (DicomFiles.Count > 0)
                {
                    SelectedDicomFile.Value = DicomFiles[0];
                }
            }
        }

        public void ChangeImage(int delta)
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
            OnSelectedDicomFileChanged?.Invoke(SelectedDicomFile.Value);
        }

        public event Action<DICOMFile> OnSelectedDicomFileChanged;

        private string[] GetFilePaths()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter =
                "DICOM ファイル (*.dcm)|*.dcm|すべてのファイル (*.*)|*.*";
            openFileDialog.Title = "DICOM ファイルを選択してください";
            openFileDialog.Multiselect = true;

            if (openFileDialog.ShowDialog() == true)
            {
                return openFileDialog.FileNames;
            }

            return null;
        }
    }
}
