using System.ComponentModel;

namespace ValoCord.Handlers;

public class StatusBindingTunnel : INotifyPropertyChanged
{
    public static readonly StatusBindingTunnel Instance = new();

    private StatusBindingTunnel()
    {
        ProgramStatusHandler.StaticPropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(ProgramStatusHandler.CurrentStatus))
            {
                OnPropertyChanged(nameof(CurrentStatus));
                OnPropertyChanged(nameof(StatusMessage));
                OnPropertyChanged(nameof(StatusColor));
            }
        };
    }

    public ProgramStatus CurrentStatus => ProgramStatusHandler.CurrentStatus;
    public string StatusMessage => CurrentStatus.StatusMessage;
    public Avalonia.Media.Color StatusColor => CurrentStatus.StatusColor;

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged(string propName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
}