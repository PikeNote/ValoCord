using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using LibVLCSharp.Shared;
using ReactiveUI;
using ValoCord.Data;
using ValoCord.Handlers;

namespace ValoCord.ViewModels;

public class VODViewerViewModel : ViewModelBase {
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

    public String PlayerTeam => gd.playerTeam;
    
    public double Progress
    { 
        get => _progress; 
        set => this.RaiseAndSetIfChanged(ref _progress, value); 
    }
    
    public bool IsPlaying
    {
        get => _isPlaying;
        set
        {
            this.RaiseAndSetIfChanged(ref _isPlaying, value);
        }
    }
    
    public string CurrentTime
    {
        get => _currentTime;
        set => this.RaiseAndSetIfChanged(ref _currentTime, value);
    }

    public string TotalDuration
    {
        get => _totalDuration;
        set => this.RaiseAndSetIfChanged(ref _totalDuration, value);
    }

    public List<RoundData> RoundDataList => gd._roundEvents;
    public string TeamWon => gd.playerTeam;
    public string GameTime => DateTimeOffset.FromUnixTimeSeconds(gd.startTime).ToString("yyyy'/MM/dd - hh:mm tt");
    public string GameDescription => $"{gd.mode} - {MapList.GetDisplayName(gd.map)}";
    public string WindowTitle => $"ValoCord - {gd.mode} ({MapList.GetDisplayName(gd.map)}) - {AgentIcons.GetAgentNames(gd.agent)}";
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
    
    public ObservableCollection<Person> Items { get; }

    public Boolean IsSeeking = false;

    public VODViewerViewModel()
    {
        Items = new ObservableCollection<Person>
        {
            new Person { Name = "1" },
            new Person { Name = "2" },
            new Person { Name = "3" },
            new Person { Name = "4" },
            new Person { Name = "5" },
            new Person { Name = "6" },
            new Person { Name = "7" },
            new Person { Name = "8" },
            new Person { Name = "9" },
            new Person { Name = "10" },
            // etc.
        };
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
        Console.WriteLine(IsPlaying);
        if (!IsPlaying)
        {
            MediaPlayer.Play();
        }
        else
        {
            MediaPlayer.Pause();
        }
    }

    public async Task DoPlay()
    {
        if (_libVLC == null || MediaPlayer == null) return;
        
        Console.WriteLine(VideoDirectory);
        using var media = new Media(
            _libVLC,
            new Uri(VideoDirectory) //new Uri("http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4"),
        );

        MediaPlayer.Play(media);
        media.Dispose();
    }
    
    public class Person
    {
        public string Name { get; set; }
        // other properties...
    }
}