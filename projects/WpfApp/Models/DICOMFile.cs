using System;
using System.IO;
using FellowOakDicom;
using FellowOakDicom.Imaging;

namespace DicomApp
{
    public class DICOMFile
    {
        private string _filePath;
        private DicomDataset _dataset;

        public string FilePath => _filePath;

        public DICOMFile(string filePath)
        {
            _filePath = filePath;
        }

        public override string ToString()
        {
            return System.IO.Path.GetFileName(FilePath);
        }

        public void Load()
        {
            try
            {
                // DicomFile オブジェクトを使用して DICOM ファイルを読み込む
                var file = DicomFile.Open(_filePath);
                _dataset = file.Dataset;

                var transferSyntax = file.Dataset.InternalTransferSyntax;
                if (transferSyntax == DicomTransferSyntax.RLELossless)
                {
                    // RLE圧縮されている
                }
            }
            catch (Exception ex)
            {
                // ファイルの読み込みに失敗した場合の例外処理
                throw new Exception($"Failed to load DICOM file: {_filePath}",
                    ex);
            }
        }

        public DicomImage GetImage()
        {
            try
            {
                // _dataset から DICOM 画像データを取得する
                var image = new DicomImage(_dataset);
                return image;
            }
            catch (Exception ex)
            {
                // 画像データの取得に失敗した場合の例外処理
                throw new Exception(
                    $"Failed to get DICOM image from file: {_filePath}", ex);
            }
        }
    }
}
