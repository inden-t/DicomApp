using System;
using System.Windows;
using Microsoft.Win32;
using DicomApp.Models;

namespace DicomApp.UseCases
{
    public class OpenDicomFileUseCase
    {
        private readonly FileManager _fileManager;

        public OpenDicomFileUseCase(FileManager fileManager)
        {
            _fileManager = fileManager;
        }

        public void Execute()
        {
            string[] filePaths = GetFilePaths();

            if (filePaths != null && filePaths.Length > 0)
            {
                _fileManager.ClearFiles();

                foreach (string filePath in filePaths)
                {
                    try
                    {
                        DICOMFile dicomFile = new DICOMFile(filePath);
                        dicomFile.Load();
                        _fileManager.AddFile(dicomFile);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            $"Error opening DICOM file: {ex.Message}", "Error",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                if (_fileManager.DicomFiles.Count > 0)
                {
                    _fileManager.SetSelectedIndex(0);
                }
            }
        }

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
