using System.ComponentModel;
using Color = Avalonia.Media.Color;

namespace ValoCord.Handlers;

public static class ProgramStatusHandler
{
    public static event PropertyChangedEventHandler? StaticPropertyChanged;
    
    public static ProgramStatus ValorantNotOpen { get; } = new ProgramStatus()
    {
        StatusMessage = "Waiting For Valorant",
        StatusColor = Color.Parse("#0096FF")
    };
    
    public static ProgramStatus WaitingForGame { get; } = new ProgramStatus()
    {
        StatusMessage = "Waiting For Game",
        StatusColor = Color.Parse("#CF9FFF")
    };
    
    public static ProgramStatus RecordingInProgress { get; } = new ProgramStatus()
    {
        StatusMessage = "Recording In Progress",
        StatusColor = Color.Parse("#FF2400")
    };

    private static ProgramStatus _currentStatus = ValorantNotOpen;

    public static ProgramStatus CurrentStatus
    {
        get => _currentStatus;
        set
        {
            _currentStatus = value;
            OnStaticPropertyChanged(nameof(CurrentStatus));
;        }
    }
    
    private static void OnStaticPropertyChanged(string propertyName)
    {
        StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));
    }
}

public class ProgramStatus
{
    public string StatusMessage { get; set; }
    public Color StatusColor { get; set; }
}