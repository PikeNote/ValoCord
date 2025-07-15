using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using CommunityToolkit.Mvvm.Input;
using LibVLCSharp.Shared;
using Unosquare.FFME.Common;
using ValoCord.Data;
using ValoCord.ViewModels;
using Wpf.Ui.Controls;

namespace ValoCord.Views;

public partial class VODViewer : FluentWindow
{
    private VODViewerViewModel _viewModel;
        
    private readonly MediaPlayer _mediaPlayer;
    
    private bool _isDragging = false;
    private bool _wasPlaying = false;
    public VODViewer(GameData gd)
    {
        InitializeComponent();
        _viewModel = new VODViewerViewModel()
        {
            gd = gd
        };
        
        Wpf.Ui.Appearance.ApplicationThemeManager.Apply(Wpf.Ui.Appearance.ApplicationTheme.Dark);
            
        DataContext = _viewModel;

        Media.Loaded += MainWindow_Opened;

    }
    
    private async void MainWindow_Opened(object? sender, System.EventArgs e)
    {
        if (DataContext is VODViewerViewModel ViewModel)
        {
            
            await Media.Open(new Uri(ViewModel.VideoDirectory));

            VideoProgress.AddHandler(Thumb.DragStartedEvent, new DragStartedEventHandler(Thumb_DragStarted), true);
            VideoProgress.AddHandler(Thumb.DragCompletedEvent, new DragCompletedEventHandler(Thumb_DragCompleted), true);
        }
            
            
    }
    
    private void Window_OnClosing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        Media.Close();
    }

    private void VideoProgress_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
    
        _isDragging = true;
        
        _wasPlaying = Media.IsPlaying;
        Media.Pause();
    }
    
    private void VideoProgress_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        SeekToMediaPosition(sender as Slider);
        
        if (_wasPlaying)
        {
            Media.Play();
        }
        
        _isDragging = false;
        _wasPlaying = false;
    }

    private void SeekToMediaPosition(Slider slider)
    {
        if (Media.IsLoaded)
        {
            Media.Position = TimeSpan.FromMilliseconds(slider.Value);
        }
    }
    
    private void VideoProgress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (!_isDragging || Media.IsPaused)
        {
            SeekToMediaPosition(sender as Slider);
        }
    }

    private void Thumb_DragStarted(object sender, DragStartedEventArgs e)
    {
        _isDragging = true;
    }
    
    private void Thumb_DragCompleted(object sender, DragCompletedEventArgs e)
    {
        _isDragging = false;

        SeekToMediaPosition(sender as Slider);
    }

    private void Media_OnMediaOpened(object? sender, MediaOpenedEventArgs e)
    {
        if (DataContext is VODViewerViewModel ViewModel)
        {
            ViewModel.MediaDuration = Media.NaturalDuration ?? TimeSpan.Zero;
        }
    }
    
    [RelayCommand]
    public void ChangeTime(object values)
    {
        if (DataContext is VODViewerViewModel ViewModel)
        {
            if (values is not RoundEvent roundEvent) return;
            Media.Position = TimeSpan.FromMilliseconds(ViewModel.gd._roundStartTimeStamps[ViewModel.SelectedRound] - ViewModel.gd.recordingStartTime - 3000 + roundEvent.TimeIntoRound);
        }
    }
}