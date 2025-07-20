using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using ValoCord.Data;
using Brushes = System.Drawing.Brushes;
using Color = System.Windows.Media.Color;
using Point = System.Windows.Point;

namespace ValoCord.Converters;

public class AgentTeamToBrushConverter: IMultiValueConverter
{
    private readonly LinearGradientBrush _currentTeamBrush = new()
    {
        StartPoint = new Point(0, 0),
        EndPoint = new Point(1, 1),
        GradientStops =
        [
            new GradientStop((Color)ColorConverter.ConvertFromString("#5569c2ae"), 0),
            new GradientStop(Colors.Transparent, 0.7)
        ]
    };
    
    private readonly LinearGradientBrush _opposingTeamBrush = new()
    {
        StartPoint = new Point(0, 0),
        EndPoint = new Point(1, 1),
        GradientStops =
        [
            new GradientStop((Color)ColorConverter.ConvertFromString("#55f25a5b"), 0),
            new GradientStop(Colors.Transparent, 0.7)
        ]
    };
    
    private readonly LinearGradientBrush _currentPlayerBrush = new()
    {
        StartPoint = new Point(0, 0),
        EndPoint = new Point(1, 1),
        GradientStops =
        [
            new GradientStop((Color)ColorConverter.ConvertFromString("#65efeeb2"), 0),
            new GradientStop(Colors.Transparent, 0.7)
        ]
    };
    
    public object? Convert(object[]? values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values is { Length: < 3 })
            return Brushes.Transparent;

        var recordingPlayerUuid = values?[3] as string;

        if (values?[0] is not string playerUuid || values[1] is not Dictionary<string, PlayerData> allPlayers ||
            values[2] is not string playerTeam || !allPlayers.TryGetValue(playerUuid, out var player) ||
            recordingPlayerUuid == null) return null;
        if (player.TeamId != playerTeam) return _opposingTeamBrush;
        return recordingPlayerUuid == playerUuid ? _currentPlayerBrush : _currentTeamBrush;
    }

    public object[] ConvertBack(object? value, Type[] targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class AgentTeamToBorderConverter: IMultiValueConverter
{
    private readonly LinearGradientBrush _currentTeamBrush = new()
    {
        StartPoint = new Point(0, 0),
        EndPoint = new Point(1, 1),
        GradientStops =
        [
            new GradientStop((Color)ColorConverter.ConvertFromString("#20ffffff"), 0),
            new GradientStop((Color)ColorConverter.ConvertFromString("#5569c2ae"), 0.7)
        ]
    };
    
    private readonly LinearGradientBrush _opposingTeamBrush = new()
    {
        StartPoint = new Point(0, 0),
        EndPoint = new Point(1, 1),
        GradientStops =
        [
            new GradientStop((Color)ColorConverter.ConvertFromString("#20ffffff"), 0),
            new GradientStop((Color)ColorConverter.ConvertFromString("#55f25a5b"), 0.7)
        ]
    };
    
    private readonly LinearGradientBrush _currentPlayerBrush = new()
    {
        StartPoint = new Point(0, 0),
        EndPoint = new Point(1, 1),
        GradientStops =
        [
            new GradientStop((Color)ColorConverter.ConvertFromString("#20ffffff"), 0),
            new GradientStop((Color)ColorConverter.ConvertFromString("#65efeeb2"), 0.7)
        ]
    };
    public object? Convert(object[]? values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values is { Length: < 3 })
            return Brushes.Transparent;

        var recordingPlayerUuid = values?[3] as string;

        if (values?[0] is not string playerUuid || values[1] is not Dictionary<string, PlayerData> allPlayers ||
            values[2] is not string playerTeam || !allPlayers.TryGetValue(playerUuid, out var player) ||
            recordingPlayerUuid == null) return null;
        if (player.TeamId != playerTeam) return _opposingTeamBrush;
        return recordingPlayerUuid == playerUuid ? _currentPlayerBrush : _currentTeamBrush;
    }

    public object[] ConvertBack(object? value, Type[] targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}