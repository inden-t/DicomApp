# DICOM App

DICOM Appは、DICOM（Digital Imaging and Communications in Medicine）形式の医療画像ファイルを閲覧・操作するためのデスクトップアプリケーションです。

## 機能

- DICOM画像ファイルの読み込みと表示
- 画像のズームイン/ズームアウト
- 3Dポイントクラウドの表示
- サーフェスモデルの表示（通常版と線形補間版）

## 技術スタック

- 言語: C#
- フレームワーク: WPF (Windows Presentation Foundation)
- .NET バージョン: .NET 8.0
- 主要ライブラリ:
  - fo-dicom: DICOMファイルの読み込みと操作
  - ReactiveProperty: リアクティブプログラミングのサポート
  - Prism.Wpf: MVVMパターンの実装サポート

## プロジェクト構成

- `Models`: データモデルとビジネスロジック
- `ViewModels`: ビューとモデルを橋渡しするビューモデル
- `Views`: ユーザーインターフェース
- `UseCases`: アプリケーションの主要な機能を実装するユースケース

## セットアップ

1. リポジトリをクローンします。
2. Visual Studioでソリューションを開きます。
3. 必要なNuGetパッケージを復元します。
4. プロジェクトをビルドし、実行します。

## 使用方法

1. アプリケーションを起動します。
2. "File" メニューから "Open File" または "Open Folder" を選択し、DICOM画像ファイルを読み込みます。
3. 左側のリストビューで表示したい画像を選択します。
4. 画像表示エリアで以下の操作が可能です：
   - マウスホイール: ズームイン/ズームアウト
   - Ctrlキー + マウスホイール: 画像の切り替え
5. メニューから "Display Point Cloud 3D" または "Display Surface Model" を選択すると、3Dモデルが表示されます。

## 開発者向け情報

- `App.xaml.cs`: アプリケーションのエントリーポイントとDI（依存性注入）の設定
- `MainWindowViewModel.cs`: メインウィンドウの主要なロジックを管理
- `ImageViewerViewModel.cs`: 画像表示に関するロジックを管理
- `DisplayPointCloud3dUseCase.cs`と`DisplaySurfaceModelUseCase.cs`: 3Dモデル生成のロジックを実装
