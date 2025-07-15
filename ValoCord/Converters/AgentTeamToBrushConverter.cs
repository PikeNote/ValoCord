using System;
using System.Collections.Generic;
using System.Drawing;
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
    private readonly LinearGradientBrush currentTeamBrush = new LinearGradientBrush
    {
        StartPoint = new Point(0, 0),
        EndPoint = new Point(1, 1),
        GradientStops = new GradientStopCollection
        {
            new GradientStop((Color)System.Windows.Media.ColorConverter.ConvertFromString("#3269c2ae"), 0),
            new GradientStop(Colors.Transparent, 0.7)
        }
    };
    
    private readonly LinearGradientBrush opposingTeamBrush = new LinearGradientBrush
    {
        StartPoint = new Point(0, 0),
        EndPoint = new Point(1, 1),
        GradientStops = new GradientStopCollection()
        {
            new GradientStop((Color)System.Windows.Media.ColorConverter.ConvertFromString("#32f25a5b"), 0),
            new GradientStop(Colors.Transparent, 0.7)
        }
    };
    
    private readonly LinearGradientBrush currentPlayerBrush = new LinearGradientBrush
    {
        StartPoint = new Point(0, 0),
        EndPoint = new Point(1, 1),
        GradientStops = new GradientStopCollection
        {
            new GradientStop((Color)System.Windows.Media.ColorConverter.ConvertFromString("#32f25a5b"), 0),
            new GradientStop(Colors.Transparent, 0.7)
        }
    };
    
    public object? Convert(object[]? values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Length < 3)
            return Brushes.Transparent;
        
        var playerUUID = values[0] as string;
        var allPlayers = values[1] as Dictionary<string, PlayerData>;
        var playerTeam = values[2] as string;
        var recordingPlayerUUID = values[3] as string;
        
        if (playerUUID != null && allPlayers != null && playerTeam != null && allPlayers.TryGetValue(playerUUID, out var player) && recordingPlayerUUID != null)
        {

            if (player.team_id == playerTeam)
            {
                if (recordingPlayerUUID == playerUUID)
                {
                    return currentPlayerBrush;
                }
                return currentTeamBrush;
            }

            return opposingTeamBrush;
        }
        return null;
    }

    public object[] ConvertBack(object? value, Type[] targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}