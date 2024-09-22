﻿using System;
using System.Windows;
using DicomApp.MainUseCases.PresenterInterface;
using DicomApp.PresenterInterface;
using DicomApp.ViewModels;

namespace DicomApp.Views
{
    public partial class ProgressWindow : Window, IProgressWindow
    {
        public ProgressWindowViewModel ViewModel { get; }

        public ProgressWindow()
        {
            InitializeComponent();

            ViewModel = new ProgressWindowViewModel();
            DataContext = ViewModel;
        }

        public void Start()
        {
            Application.Current.MainWindow!.IsEnabled = false;
            Show();
        }

        public void End()
        {
            Close();
            Application.Current.MainWindow!.IsEnabled = true;
        }

        public void SetWindowTitle(string text)
        {
            ViewModel.WindowTitle.Value = text;
        }

        public void SetStatusText(string text)
        {
            ViewModel.StatusText.Value = text;
        }

        public void SetProgress(double progress)
        {
            ViewModel.Progress.Value = progress;
        }
    }
}
