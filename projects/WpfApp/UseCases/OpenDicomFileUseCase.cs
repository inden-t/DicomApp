﻿using System.IO;
using System.Windows;
using DicomApp.BloodVesselExtraction.UseCases;
using DicomApp.CoreModels.Models;
using DicomApp.MainUseCases.PresenterInterface;
using DicomApp.WpfApp.PresenterInterface;
using Microsoft.Win32;

namespace DicomApp.WpfApp.UseCases
{
    public class OpenDicomFileUseCase
    {
        private readonly IOpenDicomFilePresenter _openDicomFilePresenter;
        private readonly IProgressWindowFactory _progressWindowFactory;
        private readonly FileManager _fileManager;

        private readonly ManageBloodVesselRegionUseCase
            _manageBloodVesselRegionUseCase;

        private IProgressWindow _progressWindow;

        public OpenDicomFileUseCase(
            IOpenDicomFilePresenter openDicomFilePresenter,
            IProgressWindowFactory progressWindowFactory,
            FileManager fileManager,
            ManageBloodVesselRegionUseCase manageBloodVesselRegionUseCase)
        {
            _openDicomFilePresenter = openDicomFilePresenter;
            _progressWindowFactory = progressWindowFactory;
            _fileManager = fileManager;
            _manageBloodVesselRegionUseCase = manageBloodVesselRegionUseCase;
        }

        public async Task ExecuteAsync()
        {
            string[] filePaths = GetFilePaths();

            if (filePaths != null && filePaths.Length > 0)
            {
                _manageBloodVesselRegionUseCase.EndRegionSelection();
                _fileManager.ClearFiles();
                await OpenFilesFromPathsAsync(filePaths);
            }
        }

        public async Task ExecuteFolderAsync()
        {
            string folderPath = GetFolderPath();

            if (!string.IsNullOrEmpty(folderPath))
            {
                _manageBloodVesselRegionUseCase.EndRegionSelection();
                _fileManager.ClearFiles();

                string[] filePaths = Directory.GetFiles(folderPath, "*.dcm",
                    SearchOption.AllDirectories);

                await OpenFilesFromPathsAsync(filePaths);
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

        private async Task OpenFilesFromPathsAsync(string[] filePaths)
        {
            try
            {
                _progressWindow = _progressWindowFactory.Create();
                _progressWindow.SetWindowTitle("ファイルを開いています");
                _progressWindow.Start();

                int totalFiles = filePaths.Length;
                var dicomFiles = new List<DICOMFile>();

                for (int i = 0; i < totalFiles; i++)
                {
                    string filePath = filePaths[i];
                    try
                    {
                        DICOMFile dicomFile = new DICOMFile(filePath);
                        await Task.Run(() => dicomFile.Load());
                        dicomFiles.Add(dicomFile);

                        double progress = (i + 1) / (double)totalFiles * 100;
                        _progressWindow.SetProgress(progress);
                        string statusText =
                            $"ファイルを読み込んでいます... ({i + 1}/{totalFiles})";
                        _progressWindow.SetStatusText(statusText);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            $"Error opening DICOM file: {ex.Message}", "Error",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                // スライス位置でソート
                var sortedDicomFiles = dicomFiles
                    .OrderBy(file => file.GetSliceLocation()).ToList();

                // ソートされたファイルをFileManagerに追加
                _fileManager.ClearFiles();
                int totalSortedFiles = sortedDicomFiles.Count;
                for (int i = 0; i < totalSortedFiles; i++)
                {
                    var dicomFile = sortedDicomFiles[i];
                    await Task.Run(() => _fileManager.AddFile(dicomFile));
                    double progress = (i + 1) / (double)totalSortedFiles * 100;
                    _progressWindow.SetProgress(progress);
                    string statusText =
                        $"画像をキャッシュしています... ({i + 1}/{totalSortedFiles})";
                    _progressWindow.SetStatusText(statusText);
                }

                if (_fileManager.DicomFiles.Count > 0)
                {
                    _openDicomFilePresenter.UpdateDisplayedImage(
                        _fileManager.DicomFiles);
                }
            }
            finally
            {
                _progressWindow.End();
            }
        }
    }
}
