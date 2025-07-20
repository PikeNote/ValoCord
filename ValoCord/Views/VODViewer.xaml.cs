using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using CommunityToolkit.Mvvm.Input;
using LibVLCSharp.Shared;
using Unosquare.FFME.Common;
using ValoCord.Data;
using ValoCord.ViewModels;
using Wpf.Ui.Controls;
using MenuItem = Wpf.Ui.Controls.MenuItem;

namespace ValoCord.Views;

public partial class VODViewer : FluentWindow
{

    private MenuItem _lastCheckedMenuItem;
    private bool _isDragging;
    private bool _wasPlaying;
    
    public VODViewer(GameData gd)
    {
        InitializeComponent();
        DataContext = new VodViewerViewModel()
        {
            gd = gd
        };
        
        Wpf.Ui.Appearance.ApplicationThemeManager.Apply(Wpf.Ui.Appearance.ApplicationTheme.Dark);

        Media.Loaded += MainWindow_Opened;

    }
    
    private async void MainWindow_Opened(object? sender, System.EventArgs e)
    {
        if (DataContext is VodViewerViewModel viewModel)
        {
            
            await Media.Open(new Uri(viewModel.VideoDirectory));

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

    private void SeekToMediaPosition(Slider? slider)
    {
        if (!Media.IsLoaded) return;
        if (slider != null) Media.Position = TimeSpan.FromMilliseconds(slider.Value);
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
        if (DataContext is VodViewerViewModel ViewModel)
        {
            ViewModel.MediaDuration = Media.NaturalDuration ?? TimeSpan.Zero;
        }
    }
    
    [RelayCommand]
    public void ChangeTime(object values)
    {
        if (DataContext is VodViewerViewModel ViewModel)
        {
            if (values is not RoundEvent roundEvent) return;
            Media.Position = TimeSpan.FromMilliseconds(ViewModel.gd.RoundStartTimeStamps[ViewModel.SelectedRound] - ViewModel.gd.RecordingStartTime - 3000 + roundEvent.TimeIntoRound);
        }
    }

    private void FrameworkElement_OnLoaded(object sender, RoutedEventArgs e)
    {
        foreach (var item in TimerControl.Items)
        {
            if (item is MenuItem { IsChecked: true } menuItem)
            {
                _lastCheckedMenuItem = menuItem;
                break;
            }
        }
    }

    private void MenuItem_OnClick(object sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem clickedMenuItem)
        {
            return;
        }

        if (clickedMenuItem == _lastCheckedMenuItem)
        {
            clickedMenuItem.IsChecked = true;
            return;
        }
        
        _lastCheckedMenuItem.IsChecked = false;
        clickedMenuItem.IsChecked = true;
        _lastCheckedMenuItem = clickedMenuItem;

        if (clickedMenuItem.Header is string header)
        {
            string playbackSpeed = header.TrimEnd('x');
            if (double.TryParse(playbackSpeed, out double newSpeed))
            {
                Media.SpeedRatio = newSpeed;
                PlaybackSelector.Content = header;
            }
        }
        else
        {
            double newPlaybackSpeed = Math.Round(PlaybackSpeedSlider.Value, 2);
            Media.SpeedRatio = newPlaybackSpeed;
            PlaybackSelector.Content = $"{newPlaybackSpeed}x";
        }
    }

    private void PlaybackSpeedSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (_lastCheckedMenuItem.Header is string)
        {
            foreach (var item in TimerControl.Items)
            {
                if (item is MenuItem { Header: StackPanel } menuItem)
                {
                    MenuItem_OnClick(menuItem, new RoutedEventArgs());
                    break;
                }
            }
        }
        
        double newPlaybackSpeed = Math.Round(e.NewValue,2);
        Media.SpeedRatio = newPlaybackSpeed;
        PlaybackSelector.Content = $"{newPlaybackSpeed}x";
    }
}