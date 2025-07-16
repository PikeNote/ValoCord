using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ValoCord.Handlers;

public class ProgramStatusHandler : ObservableObject
{
    public static ProgramStatus ValorantNotOpen { get; } = new()
    {
        StatusMessage = "Waiting For Valorant",
        StatusColor =  CreateFrozenBrush("#0096FF")
       
    };
    
    public static ProgramStatus WaitingForGame { get; } = new()
    {
        StatusMessage = "Waiting For Game",
        StatusColor = CreateFrozenBrush("#CF9FFF")
    };
    
    public static ProgramStatus RecordingInProgress { get; } = new()
    {
        StatusMessage = "Recording In Progress",
        StatusColor = CreateFrozenBrush("#FF2400")
    };
    
    public static readonly ProgramStatusHandler Instance = new();

    private ProgramStatus _currentStatus = ValorantNotOpen;
    public ProgramStatus CurrentStatus
    {
        get => _currentStatus;
        set
        {
            if (SetProperty(ref _currentStatus, value))
            {
                OnPropertyChanged(nameof(StatusMessage));
                OnPropertyChanged(nameof(StatusColorBrush));
            }
        }
    }
    
    public string StatusMessage => CurrentStatus.StatusMessage;
    
    public SolidColorBrush StatusColorBrush => CurrentStatus.StatusColor;
    private static SolidColorBrush CreateFrozenBrush(string hexColor)
    {
        var brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(hexColor));
        brush.Freeze();
        return brush;
    }

    private ProgramStatusHandler()
    {
        CurrentStatus = ValorantNotOpen;
    }

}

public class ProgramStatus
{
    public required string StatusMessage { get; set; }
    public required SolidColorBrush StatusColor { get; set; }
}