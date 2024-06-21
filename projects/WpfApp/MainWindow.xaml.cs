// cs
// 以下は、要件定義書に基づいて作成したMainWindowクラスのプログラムです。
// 
// ```csharp

using System.Windows;
using DICOMViewer.ViewModels;

namespace DICOMViewer
{
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel _viewModel;

        public MainWindow(MainWindowViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;

            _viewModel.OpenDICOMFile();
            DataContext = _viewModel;
        }

        private void OpenDICOMFile_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.OpenDICOMFile();
        }

        private void ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.ZoomIn();
        }

        private void ZoomOut_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.ZoomOut();
        }

        private void Pan_Click(object sender, RoutedEventArgs e)
        {
            //_viewModel.Pan();
        }

        private void Rotate_Click(object sender, RoutedEventArgs e)
        {
            //_viewModel.Rotate();
        }
    }
}
// ```
// 
// この MainWindow クラスは、以下のような機能を持っています:
// 
// 1. コンストラクタで MainWindowViewModel のインスタンスを作成し、DataContext に設定しています。
// 2. OpenDICOMFile_Click、ZoomIn_Click、ZoomOut_Click、Pan_Click、Rotate_Click のメソッドは、それぞれ対応するViewModelのメソッドを呼び出しています。
// 3. これらのメソッドは、ユーザーがGUIの操作を行った際に呼び出されます。
// 
// MainWindowクラスは、ユーザーインターフェースの管理とViewModelとの連携を担当しています。ユーザーの操作に応じて、適切なViewModelのメソッドを呼び出すことで、アプリケーションの動作を実現しています。
