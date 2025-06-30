    using System;
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Controls.Primitives;
    using Avalonia.Input;
    using Avalonia.Interactivity;
    using FluentAvalonia.UI.Windowing;
    using LibVLCSharp.Shared;
    using ValoCord.Data;
    using ValoCord.Handlers;
    using ValoCord.ViewModels;

    namespace ValoCord.Views;

    public partial class VODViewer : AppWindow
    {
        private VODViewerViewModel _viewModel;
        
        private readonly MediaPlayer _mediaPlayer;
        
        public VODViewer(GameData gd)
        {
            InitializeComponent();
            
            _viewModel = new VODViewerViewModel()
            {
                gd = gd
            };
            
            DataContext = _viewModel;


            VODVideoView.Loaded += MainWindow_Opened;
            VideoProgress.AddHandler(PointerPressedEvent, InputElement_OnPointerPressed, RoutingStrategies.Tunnel);
            VideoProgress.AddHandler(PointerReleasedEvent, InputElement_OnPointerReleased, RoutingStrategies.Tunnel);
            TitleBar.ExtendsContentIntoTitleBar = true;
        }

        private async void MainWindow_Opened(object? sender, System.EventArgs e)
        {
            if (DataContext is VODViewerViewModel ViewModel)
            {
                if (VODVideoView != null && ViewModel!.MediaPlayer != null)
                {
                    VODVideoView.MediaPlayer = ViewModel.MediaPlayer;

                    ViewModel.PrepareVideoAsync();
                }
            }
            
            
        }

        private void InputElement_OnPointerPressed(object? sender, PointerPressedEventArgs  e)
        {
            if (_viewModel.MediaPlayer.IsPlaying)
            {
                _viewModel.MediaPlayer.Pause();
                _viewModel.IsSeeking = true;
            }
        }

        private void InputElement_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            if (!_viewModel.MediaPlayer.IsPlaying)
            {
                _viewModel.IsSeeking = false;
                _viewModel.MediaPlayer.Pause();
            }
            
        }
        
        
        private void RangeBase_OnValueChanged(object? sender, RangeBaseValueChangedEventArgs e)
        {
            if (_viewModel.IsSeeking)
            {
                _viewModel.MediaPlayer.Position = (float) e.NewValue;
            }
        }


        private void Visual_OnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Window_OnClosing(object? sender, WindowClosingEventArgs e)
        {
            _viewModel.MediaPlayer?.Stop();
            _viewModel.MediaPlayer = null;
        }
    }