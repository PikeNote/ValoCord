using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using LibVLCSharp.Shared;
using ReactiveUI;
using ValoCord.Data;
using ValoCord.Handlers;

namespace ValoCord.ViewModels;

public class VODViewerViewModel : ViewModelBase, INotifyPropertyChanged {
    private double _progress = 0.0;
    private bool _isPlaying;
    private MediaPlayer? _mediaPlayer;
    
    public LibVLC? _libVLC {get; set;}
    public MediaPlayer? MediaPlayer
    {
        get => _mediaPlayer;
        set
        {
            this.RaiseAndSetIfChanged(ref _mediaPlayer, value);
            InitializeMediaPlayerEvents(value);
        }
    }
    
    private string _currentTime = "00:00";
    private string _totalDuration = "00:00";
    
    public GameData gd {get; set;}
    
    private bool _isVideoLoading = true;
    public bool IsVideoLoading
    {
        get => _isVideoLoading;
        private set
        {
            this.RaiseAndSetIfChanged(ref _isVideoLoading, value); 
            OnPropertyChanged(nameof(IsVideoLoading));
        }

    }
    
    public double Progress
    {
        get => _progress;
        set
        {
            this.RaiseAndSetIfChanged(ref _progress, value);
            OnPropertyChanged(nameof(Progress));
        }
    }

    public bool IsPlaying
    {
        get => _isPlaying;
        set
        {
            this.RaiseAndSetIfChanged(ref _isPlaying, value);
            OnPropertyChanged(nameof(IsPlaying));
        }
    }
    
    public string CurrentTime
    {
        get => _currentTime;
        set
        {
            this.RaiseAndSetIfChanged(ref _currentTime, value);
            OnPropertyChanged(nameof(CurrentTime));
        }
    }

    public string TotalDuration
    {
        get => _totalDuration;
        set
        {
            this.RaiseAndSetIfChanged(ref _totalDuration, value);
            OnPropertyChanged(nameof(TotalDuration));
        }
    }

    public int _selectedRound = 0;
    public int SelectedRound
    {
        get => _selectedRound;
        set
        {
            if (_selectedRound == value) return;
            _selectedRound = value;
            OnPropertyChanged(nameof(CurrentRound));
        }
    }

    public List<RoundData> RoundDataList => gd._roundEvents;
    public string TeamWon => gd.playerTeam;
    public string PlayerTeam => gd.playerTeam;
    public ReactiveCommand<IList<object>, Unit> ChangeTimeCommand { get; }
    public string GameTime => DateTimeOffset.FromUnixTimeMilliseconds(gd.matchStartTime).ToString("yyyy/MM/dd - hh:mm tt");
    public string GameDescription => $"{GameMode} - {MapData.GetDisplayName(gd.map)}";
    public string WindowTitle => $"ValoCord - {GameMode} ({MapData.GetDisplayName(gd.map)}) - {AgentData.GetAgentNames(gd.agent)}";
    private string GameMode => GameModes.ConvertGameMode(gd.mode);
    public Dictionary<string, PlayerData> AllPlayers => gd._players;

    public RoundData CurrentRound => gd._roundEvents[_selectedRound];
    
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    private string FormatTime(long timeInMilliseconds)
    {
        TimeSpan timeSpan = TimeSpan.FromMilliseconds(timeInMilliseconds);
        
        if (timeSpan.TotalHours >= 1)
        {
            return timeSpan.ToString(@"hh\:mm\:ss");
        }
        else
        {
            return timeSpan.ToString(@"mm\:ss");
        }
    }

    public string VideoDirectory => Paths.generateVideoPath(gd.matchId);

    public Boolean IsSeeking = false;

    public VODViewerViewModel()
    {
        ChangeTimeCommand = ReactiveCommand.Create<IList<object>>(ChangeTime);
    }
    
    public void RoundChanged(object roundNum)
    {
        if (roundNum is not int num) return;
        SelectedRound = num - 1;
    }
    
    private void InitializeMediaPlayerEvents(MediaPlayer? player)
    {
        if (player == null) return;
        
        Observable.FromEventPattern<MediaPlayerPositionChangedEventArgs>(
                h => player.PositionChanged += h,
                h => player.PositionChanged -= h)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(x =>
            {
                CurrentTime = FormatTime(player.Time);
                if (!IsSeeking)
                {
                    Progress = x.EventArgs.Position;
                }
            });

        Observable.FromEventPattern<EventArgs>(h => player.Playing += h, h => player.Playing -= h)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(_ =>
            {
                IsPlaying = true;
                TotalDuration = FormatTime(player.Length);
            });
        
        Observable.FromEventPattern<EventArgs>(h => player.Paused += h, h => player.Paused -= h)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(_ => IsPlaying = false);
            
        Observable.FromEventPattern<EventArgs>(h => player.EndReached += h, h => player.EndReached -= h)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(_ => IsPlaying = false);
    }

    public void TogglePlayPause()
    {
        if (!IsPlaying)
        {
            MediaPlayer.Play();
        }
        else
        {
            MediaPlayer.Pause();
        }
    }

    public async Task PrepareVideoAsync()
    {
        if (_libVLC == null || MediaPlayer == null) return;
        
        IsVideoLoading = true;

        try
        {
            Media? media = null;
            await Task.Run(() => { media = new Media(_libVLC, new Uri(VideoDirectory)); });

            if (media != null)
            {
                MediaPlayer.Play(media);
                MediaPlayer.Pause(); // Start paused
                media.Dispose();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        finally
        {
            IsVideoLoading = false;
        }
    }

    public void ChangeTime(IList<object>? values)
    {
        if (values[0] is not GameKill roundKill) return;
        if (values[1] is not int roundNum) return;
        if (_mediaPlayer != null) _mediaPlayer.Position = (gd._roundStartTimeStamps[roundNum] - gd.recordingStartTime - 2000 + roundKill.TimeKillIntoRound)  / _mediaPlayer.Length;
    }
}