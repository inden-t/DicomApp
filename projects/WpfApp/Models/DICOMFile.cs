// cs
// 以下のように、DICOMFile クラスの実装を行います。
// 
// ```csharp

using System;
using System.IO;
using FellowOakDicom;
using FellowOakDicom.Imaging;

public class DICOMFile
{
    private string _filePath;
    private DicomDataset _dataset;

    public DICOMFile(string filePath)
    {
        _filePath = filePath;
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
            throw new Exception($"Failed to load DICOM file: {_filePath}", ex);
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
// ```
// 
// このクラスの主な機能は以下の通りです:
// 
// 1. コンストラクタ `DICOMFile(string filePath)`: DICOM ファイルのパスを受け取り、クラスのプロパティに設定します。
// 
// 2. `Load()` メソッド: DICOM ファイルを読み込み、`DicomDataset` オブジェクトを取得します。ファイルの読み込みに失敗した場合は例外を投げます。
// 
// 3. `GetImage()` メソッド: `DicomDataset` から DICOM 画像データを取得し、`DicomImage` オブジェクトを返します。画像データの取得に失敗した場合は例外を投げます。
// 
// このクラスは、DICOM ファイルの読み込みと画像データの取得を担当します。他のクラス (例えば `MainWindowViewModel`) がこのクラスを使用して、DICOM 画像を表示したり操作したりすることができます。
