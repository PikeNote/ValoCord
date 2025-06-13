using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using ValoCord.Data;

namespace ValoCord.Converters;

public class AgentTeamToBrushConverter: IMultiValueConverter
{
    private readonly LinearGradientBrush currentTeamBrush = new LinearGradientBrush
    {
        StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
        EndPoint = new RelativePoint(1, 1, RelativeUnit.Relative),
        GradientStops = new GradientStops
        {
            new GradientStop(Color.Parse("#3269c2ae"), 0),
            new GradientStop(Colors.Transparent, 0.7)
        }
    };
    
    private readonly LinearGradientBrush opposingTeamBrush = new LinearGradientBrush
    {
        StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
        EndPoint = new RelativePoint(1, 1, RelativeUnit.Relative),
        GradientStops = new GradientStops
        {
            new GradientStop(Color.Parse("#32f25a5b"), 0),
            new GradientStop(Colors.Transparent, 0.7)
        }
    };
    
    public object? Convert(IList<object>? values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count < 3)
            return Brushes.Transparent;
        
        var playerUUID = values[0] as string;
        var allPlayers = values[1] as Dictionary<string, PlayerData>;
        var playerTeam = values[2] as string;
        
        if (playerUUID != null && allPlayers != null && playerTeam != null && allPlayers.TryGetValue(playerUUID, out var player))
        {

            if (player.team_id == playerTeam)
            {
                return currentTeamBrush;
            }

            return opposingTeamBrush;
        }
        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}