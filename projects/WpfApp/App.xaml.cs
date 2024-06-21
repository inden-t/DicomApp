﻿using System;
using System.Windows;
using DICOMViewer.ViewModels;
using DICOMViewer.Views;
using FellowOakDicom;
using FellowOakDicom.Imaging;
using Microsoft.Extensions.DependencyInjection;

namespace DICOMViewer
{
    public partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;

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

            // MainWindow の生成
            _mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            _mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // クラスの登録
            services.AddScoped<MainWindow>();
            services.AddScoped<MainWindowViewModel>();
            services.AddScoped<DICOMFile>();
            services.AddScoped<ImageViewer>();
            services.AddScoped<ImageViewerViewModel>();
        }
    }
}
