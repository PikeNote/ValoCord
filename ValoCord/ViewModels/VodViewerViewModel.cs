using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Unosquare.FFME.Common;
using ValoCord.Data;
using ValoCord.Handlers;

namespace ValoCord.ViewModels;

public partial class VodViewerViewModel : ViewModelBase, INotifyPropertyChanged {
    
    [ObservableProperty]
    private double _progress = 0.0;
    
    [ObservableProperty]
    private bool _isPlaying;
    
    [ObservableProperty]
    private string _currentTime = "00:00";
    
    [ObservableProperty]
    private string _totalDuration = "00:00";
    
    public required GameData gd {get; set;}
    
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
    
   

    public List<RoundData> RoundDataList => gd.RoundEvents;
    public string TeamWon => gd.PlayerTeam;
    public string PlayerTeam => gd.PlayerTeam;
    public string PlayerRecordingUUID => gd.PlayerUuid;
    public string GameTime => DateTimeOffset.FromUnixTimeMilliseconds(gd.MatchStartTime).ToLocalTime().ToString("yyyy/MM/dd - hh:mm tt");
    public string GameDescription => $"{GameMode} - {MapData.GetDisplayName(gd.Map)}";
    public string WindowTitle => $"ValoCord - {GameMode} ({MapData.GetDisplayName(gd.Map)}) - {AgentData.GetAgentNames(gd.Agent)}";
    private string GameMode => GameModes.ConvertGameMode(gd.Mode);
    public Dictionary<string, PlayerData> AllPlayers => gd.Players;

    public RoundData CurrentRound => gd.RoundEvents[_selectedRound];
    
    [ObservableProperty]
    private TimeSpan _mediaPosition;
    
    [ObservableProperty]
    private TimeSpan _mediaDuration;

    
    public string VideoDirectory => Paths.generateVideoPath(gd.MatchId);

    public Boolean IsSeeking = false;
    
    [RelayCommand]
    private void RoundChanged(object roundNum)
    {
        if (roundNum is not int num) return;
        SelectedRound = num - 1;
    }

    [RelayCommand]
    private void TogglePlayPause(Unosquare.FFME.MediaElement mediaElement)
    {
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
        Console.WriteLine(gd.RoundStartTimeStamps[SelectedRound] - gd.RecordingStartTime - 3000 + roundKill.TimeIntoRound);
        MediaPosition = TimeSpan.FromMilliseconds(gd.RoundStartTimeStamps[SelectedRound] - gd.RecordingStartTime - 3000 + roundKill.TimeIntoRound);
    }
}