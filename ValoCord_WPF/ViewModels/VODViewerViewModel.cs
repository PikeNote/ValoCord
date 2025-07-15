using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LibMPVSharp;
using LibVLCSharp.Shared;
using ReactiveUI;
using Unosquare.FFME.Common;
using ValoCord_WPF.Data;
using ValoCord_WPF.Handlers;

namespace ValoCord_WPF.ViewModels;

public partial class VODViewerViewModel : ViewModelBase, INotifyPropertyChanged {
    
    [ObservableProperty]
    private double _progress = 0.0;
    
    [ObservableProperty]
    private bool _isPlaying;
    
    [ObservableProperty]
    private string _currentTime = "00:00";
    
    [ObservableProperty]
    private string _totalDuration = "00:00";
    
    public GameData gd {get; set;}
    
    [ObservableProperty]
    private bool _isVideoLoading = true;
    

    private int _selectedRound = 0;
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
    public string PlayerRecordingUUID => gd.playerUUID;
    public string GameTime => DateTimeOffset.FromUnixTimeMilliseconds(gd.matchStartTime).ToString("yyyy/MM/dd - hh:mm tt");
    public string GameDescription => $"{GameMode} - {MapData.GetDisplayName(gd.map)}";
    public string WindowTitle => $"ValoCord - {GameMode} ({MapData.GetDisplayName(gd.map)}) - {AgentData.GetAgentNames(gd.agent)}";
    private string GameMode => GameModes.ConvertGameMode(gd.mode);
    public Dictionary<string, PlayerData> AllPlayers => gd._players;

    public RoundData CurrentRound => gd._roundEvents[_selectedRound];
    
    public event PropertyChangedEventHandler PropertyChanged;
    
    [ObservableProperty]
    private TimeSpan _mediaPosition;
    
    [ObservableProperty]
    private TimeSpan _mediaDuration;



    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public string VideoDirectory => Paths.generateVideoPath(gd.matchId);

    public Boolean IsSeeking = false;

    public VODViewerViewModel() { }
    
    [RelayCommand]
    public void RoundChanged(object roundNum)
    {
        if (roundNum is not int num) return;
        SelectedRound = num - 1;
    }

    [RelayCommand]
    private void TogglePlayPause(Unosquare.FFME.MediaElement mediaElement)
    {
        if (mediaElement == null) return;
        
        if (mediaElement.MediaState == MediaPlaybackState.Play)
        {
            mediaElement.Pause();
        }
        else
        {
            mediaElement.Play();
        }
    }
    
    [RelayCommand]
    private void ChangeTime(object values)
    {
        if (values is not GameKill roundKill) return;
        Console.WriteLine(gd._roundStartTimeStamps[SelectedRound] - gd.recordingStartTime - 3000 + roundKill.TimeIntoRound);
        MediaPosition = TimeSpan.FromMilliseconds(gd._roundStartTimeStamps[SelectedRound] - gd.recordingStartTime - 3000 + roundKill.TimeIntoRound);
    }
}