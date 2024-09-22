using FellowOakDicom;
using FellowOakDicom.Imaging;

namespace DicomApp.CoreModels.Models
{
    public class DICOMFile
    {
        private readonly string _filePath;
        private DicomDataset _dataset;
        private DicomImage _image;

        public string FilePath => _filePath;

        public DICOMFile(string filePath)
        {
            _filePath = filePath ??
                        throw new ArgumentNullException(nameof(filePath));
        }

        public override string ToString()
        {
            string text =
                $"{System.IO.Path.GetFileName(_filePath)} ({_filePath})";
            return text;
        }

        public void Load()
        {
            try
            {
                // DicomFile オブジェクトを使用して DICOM ファイルを読み込む
                var file = DicomFile.Open(_filePath);
                _dataset = file.Dataset;

                // _dataset から DICOM 画像データを取得し、フィールドに保持する
                _image = new DicomImage(_dataset);

                var transferSyntax = file.Dataset.InternalTransferSyntax;
                if (transferSyntax == DicomTransferSyntax.RLELossless)
                {
                    // RLE圧縮されている場合の処理
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
            if (_image == null)
            {
                throw new InvalidOperationException(
                    "DICOM画像がロードされていません。最初にLoad()を呼び出してください。");
            }

            return _image;
        }
    }
}
