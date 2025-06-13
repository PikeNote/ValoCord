using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using ValoCord.Data;

namespace ValoCord.Converters;

public class AgentIconNameConverter : IMultiValueConverter
{
    public object? Convert(IList<object>? values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count < 2 || !(values[0] is string agentUUID) || string.IsNullOrEmpty(agentUUID) ||
            !(values[1] is Dictionary<String, PlayerData> players))
        {
            var uri = new Uri($"avares://Valocord{AgentData.GetAgentIcons(
                AgentData.GetAgentNames("Jett")
            )}");
            return new Bitmap(AssetLoader.Open(uri));
        }
        
        try
        {
            var player = players[agentUUID];
            var uri = new Uri($"avares://Valocord{AgentData.GetAgentIcons(
                AgentData.GetAgentNames(player.character_played)
            )}");
            return new Bitmap(AssetLoader.Open(uri));
        }
        catch (KeyNotFoundException ex)
        {
            return null;
        }

        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}