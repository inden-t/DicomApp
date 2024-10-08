﻿using System.Windows;
using DicomApp.BloodVesselExtraction.Models;
using DicomApp.BloodVesselExtraction.Presenter;
using DicomApp.BloodVesselExtraction.PresenterInterface;
using DicomApp.BloodVesselExtraction.UseCases;
using DicomApp.BloodVesselExtraction.ViewModels;
using DicomApp.BloodVesselExtraction.Views;
using DicomApp.CoreModels.Models;
using DicomApp.MainUseCases.PresenterInterface;
using DicomApp.MainUseCases.UseCases;
using DicomApp.WpfApp.Presenter;
using DicomApp.WpfApp.PresenterInterface;
using DicomApp.WpfApp.UseCases;
using DicomApp.WpfApp.ViewModels;
using DicomApp.WpfApp.Views;
using FellowOakDicom;
using FellowOakDicom.Imaging;
using Microsoft.Extensions.DependencyInjection;

namespace DicomApp.WpfApp
{
    public partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;

        private DependencyInitializer _dependencyInitializer;
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
            _dependencyInitializer =
                _serviceProvider.GetRequiredService<DependencyInitializer>();

            // MainWindow の設定と表示
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
            services.AddScoped<DependencyInitializer>();
            services.AddScoped<MainWindow>();
            services
                .AddScoped<IOpenDicomFilePresenter, OpenDicomFilePresenter>();
            services.AddScoped<MainWindowViewModel>();
            services.AddScoped<IModel3dViewer, Model3dViewer>();
            services.AddScoped<IProgressWindow, ProgressWindow>();
            services.AddScoped<FileManager>();
            services.AddScoped<IImageCaches, ImageCaches>();
            services.AddScoped<DICOMFile>();
            services.AddScoped<ImageViewer>();
            services.AddScoped<ImageViewerViewModel>();

            services.AddScoped<OpenDicomFileUseCase>();
            services.AddScoped<SaveModel3dUseCase>();
            services.AddScoped<LoadModel3dUseCase>();
            services.AddScoped<GeneratePointCloudUseCase>();
            services.AddScoped<GenerateSurfaceModelUseCase>();
            services
                .AddScoped<GenerateSurfaceModelLinearInterpolationUseCase>();

            services.AddScoped<SelectionOverlayControl>();
            services.AddScoped<SelectionOverlayControlViewModel>();
            services.AddScoped<BloodVesselExtractionRibbonTab>();
            services.AddScoped<IManageBloodVesselRegionPresenter,
                ManageBloodVesselRegionPresenter>();
            services.AddScoped<BloodVesselExtractionRibbonTabViewModel>();
            services.AddScoped<BloodVesselExtractionUseCase>();
            services.AddScoped<Select3DBloodVesselRegionUseCase>();
            services.AddScoped<ManageBloodVesselRegionUseCase>();
            services.AddScoped<BloodVessel3DRegionSelector>();
            services.AddScoped<BloodVesselSurfaceModelGenerator>();

            services.AddScoped<IModel3dViewerFactory, Model3dViewerFactory>();
            services.AddScoped<IProgressWindowFactory, ProgressWindowFactory>();
        }
    }
}
