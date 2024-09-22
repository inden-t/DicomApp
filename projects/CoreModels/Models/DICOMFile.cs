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

        public double GetSliceLocation()
        {
            if (_dataset.Contains(DicomTag.SliceLocation))
            {
                return _dataset.GetSingleValue<double>(DicomTag.SliceLocation);
            }
            else if (_dataset.Contains(DicomTag.ImagePositionPatient))
            {
                // ImagePositionPatientの3番目の値（Z軸）をスライス位置として使用
                var position =
                    _dataset.GetValues<double>(DicomTag.ImagePositionPatient);
                return position[2];
            }
            else
            {
                // スライス位置情報が見つからない場合は、適切なデフォルト値または例外処理を行う
                return 0;
            }
        }
    }
}
