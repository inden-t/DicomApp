# DICOM App

DICOM Appは、DICOM（Digital Imaging and Communications in Medicine）形式の医療画像ファイルを閲覧・操作するためのデスクトップアプリケーションです。

## 機能

### 2D画像表示機能
- DICOM画像ファイルの読み込みと表示
- 画像のズームイン/ズームアウト
- 画像のパン（移動）

### 3Dモデル表示機能
- 高輝度領域の3D可視化
  - 3D点群表示
  - サーフェスモデル表示
    - マーチングキューブ法による生成
    - 線形補間オプション（あり/なし）
- 3Dモデル操作
  - 回転
  - ズーム
  - パン（上下左右の移動）
  - ドリー（前後の移動）

## 技術スタック

- 言語: C#
- フレームワーク: WPF (Windows Presentation Foundation)
- .NET バージョン: .NET 8.0
- 主要ライブラリ:
  - fo-dicom: DICOMファイルの読み込みと操作
  - ReactiveProperty: リアクティブプログラミングのサポート
  - Prism.Wpf: MVVMパターンの実装サポート

## セットアップ

1. リポジトリをクローンします。
2. Visual Studioでソリューションを開きます。
3. 必要なNuGetパッケージを復元します。
4. プロジェクトをビルドし、実行します。

## 使用方法

### 2D画像表示
1. アプリケーションを起動します。
2. DICOM画像ファイルを含むフォルダを選択し、ファイルを読み込みます。（フォルダ内の全てのDICOMファイルが読み込まれます）
3. ファイルを読み込まなかった場合は "File" メニューから "Open File" または "Open Folder" を選択し、DICOM画像ファイルを読み込みます。
   - "Open File" ではファイルを複数選択して開くことができます。
4. 左側のリストビューで表示したい画像を選択します。
5. 画像表示エリアで以下の操作が可能です：
   - マウスホイール: スライスの移動（前後の画像に切り替え）
   - Ctrlキー + マウスホイール: ズームイン/ズームアウト
   - マウス中ボタンドラッグ: 画像のパン（移動）

### 3Dモデル表示
1. メニューから "Display Point Cloud 3D" または "Display Surface Model" または "(線形補間)" を選択すると、3Dモデルが表示されます。
2. 3Dモデル表示ウィンドウでは以下の操作が可能です：
   - マウス左ボタンドラッグ: 回転
   - マウスホイール: ズームイン/ズームアウト
   - マウス中ボタンドラッグ: 上下左右の移動
   - Shiftキー + マウスホイール: 前後の移動
3. 3Dビューワーの中央には黄色い点が表示され、回転の中心を示します。

## 開発者向け情報

### 主要なクラスとファイル
- `App.xaml.cs`: アプリケーションのエントリーポイントとDI（依存性注入）の設定
- `MainWindowViewModel.cs`: メインウィンドウの主要なロジックを管理
- `ImageViewerViewModel.cs`: 画像表示に関するロジックを管理
- `DisplayPointCloud3dUseCase.cs`: 3D点群モデル生成のロジックを実装
- `DisplaySurfaceModelUseCase.cs`: サーフェスモデル生成のロジックを実装
- `Model3dViewer.xaml.cs`: 3Dモデルの表示と操作に関するロジックを実装
- `ProgressWindow.xaml.cs`: プログレスウィンドウの表示と管理を実装

### プロジェクト構成
- `Models`: データモデルとビジネスロジック
  - 例: `DICOMFile.cs`
- `ViewModels`: ビューとモデルを橋渡しするビューモデル
  - 例: `MainWindowViewModel.cs`, `ImageViewerViewModel.cs`
- `Views`: ユーザーインターフェース
  - 例: `MainWindow.xaml`, `ImageViewer.xaml`, `Model3dViewer.xaml`
- `UseCases`: アプリケーションの主要な機能を実装するユースケース
  - 例: `OpenDicomFileUseCase.cs`, `DisplayPointCloud3dUseCase.cs`, `DisplaySurfaceModelUseCase.cs`

## 注意事項

このアプリケーションは開発者の学習目的で開発されたものであり、実際の医療診断や臨床使用を意図したものではありません。医療目的での使用は推奨されません。開発者は、このアプリケーションの使用によって生じるいかなる損害や問題に対しても責任を負いません。

使用者は、このアプリケーションを使用する前に、適用される法律、規制、およびプライバシーポリシーを確認し、遵守する責任があります。また、DICOMファイルに含まれる個人情報やセンシティブな医療情報の取り扱いには十分注意してください。
