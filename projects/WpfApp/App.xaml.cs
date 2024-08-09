using System;
using System.Windows;
using DicomApp.Models;
using DicomApp.UseCases;
using DicomApp.ViewModels;
using DicomApp.Views;
using FellowOakDicom;
using FellowOakDicom.Imaging;
using Microsoft.Extensions.DependencyInjection;

namespace DicomApp
{
    public partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;

        private CommandFactory _commandFactory;
        private MainWindow _mainWindow;

        public App()
        {
            new DicomSetupBuilder().RegisterServices(s => s
                    .AddFellowOakDicom()
                    // DicomSetupBuilderを使ってコーデックを登録
                    .AddTranscoderManager<FellowOakDicom.Imaging.NativeCodec.
                        NativeTranscoderManager>()
                    // WPFImageManagerの登録
                    .AddImageManager<WPFImageManager>())
                .SkipValidation()
                .Build();

            // DIコンテナの設定
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // コマンド設定
            _commandFactory =
                _serviceProvider.GetRequiredService<CommandFactory>();

            // MainWindow の生成
            _mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            Current.MainWindow = _mainWindow;
            _mainWindow.Show();

            // 起動時にファイルを開く
            MainWindowViewModel _viewModel =
                _serviceProvider.GetRequiredService<MainWindowViewModel>();
            _viewModel.OpenDicomFolderCommand.Execute(null);
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // クラスの登録
            services.AddScoped<CommandFactory>();
            services.AddScoped<MainWindow>();
            services.AddScoped<IMainWindowPresenter, MainWindowPresenter>();
            services.AddScoped<MainWindowViewModel>();
            services
                .AddScoped<IBloodVesselPointCloud3DViewer,
                    BloodVesselPointCloud3DViewer>();
            services.AddScoped<IProgressWindow, ProgressWindow>();
            services.AddScoped<FileManager>();
            services.AddScoped<IImageCaches, ImageCaches>();
            services.AddScoped<DICOMFile>();
            services.AddScoped<ImageViewer>();
            services.AddScoped<ImageViewerViewModel>();

            services.AddScoped<OpenDicomFileUseCase>();
            services.AddScoped<MakeBloodVesselPointCloud3DUseCase>();
            services.AddScoped<MakeBloodVesselSurfaceModelUseCase>();
            services
                .AddScoped<IBloodVesselPointCloud3DViewerFactory,
                    BloodVesselPointCloud3DViewerFactory>();
            services.AddScoped<IProgressWindowFactory, ProgressWindowFactory>();
        }
    }
}
