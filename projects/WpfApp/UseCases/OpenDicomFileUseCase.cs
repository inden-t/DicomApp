using System;
using System.Windows;
using Microsoft.Win32;
using DicomApp.Models;
using System.IO;

namespace DicomApp.UseCases
{
    public class OpenDicomFileUseCase
    {
        private readonly IMainWindowPresenter _mainWindowPresenter;
        private readonly FileManager _fileManager;

        public OpenDicomFileUseCase(IMainWindowPresenter mainWindowPresenter,
            FileManager fileManager)
        {
            _mainWindowPresenter = mainWindowPresenter;
            _fileManager = fileManager;
        }

        public void Execute()
        {
            string[] filePaths = GetFilePaths();

            if (filePaths != null && filePaths.Length > 0)
            {
                _fileManager.ClearFiles();
                OpenFilesFromPaths(filePaths);
            }
        }

        public void ExecuteFolder()
        {
            string folderPath = GetFolderPath();

            if (!string.IsNullOrEmpty(folderPath))
            {
                _fileManager.ClearFiles();

                string[] filePaths = Directory.GetFiles(folderPath, "*.dcm",
                    SearchOption.AllDirectories);

                OpenFilesFromPaths(filePaths);
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

        private string GetFolderPath()
        {
            var dialog = new OpenFolderDialog
            {
                Title = "DICOMファイルを含むフォルダを選択してください"
            };

            if (dialog.ShowDialog() == true)
            {
                return dialog.FolderName;
            }

            return null;
        }

        private void OpenFilesFromPaths(string[] filePaths)
        {
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
                _mainWindowPresenter.UpdateDisplayedImage(
                    _fileManager.DicomFiles);
            }
        }
    }
}
