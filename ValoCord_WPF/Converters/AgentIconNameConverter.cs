using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using ValoCord_WPF.Data;


namespace ValoCord_WPF.Converters;

public class AgentIconNameConverter : IMultiValueConverter
{
    public object? Convert(object[] values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Length < 2 || !(values[0] is string agentUUID) || string.IsNullOrEmpty(agentUUID) ||
            !(values[1] is Dictionary<String, PlayerData> players))
        {
            var uri = new Uri($"pack://application:,,,{AgentData.GetAgentIcons(
                AgentData.GetAgentNames("Jett")
            )}");
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.DecodePixelWidth = 50;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.UriSource = uri;
            bitmapImage.EndInit();
            bitmapImage.Freeze(); 
            return bitmapImage;
        }
        
        try
        {
            var player = players[agentUUID];
            var uri = new Uri($"pack://application:,,,{AgentData.GetAgentIcons(
                AgentData.GetAgentNames(player.character_played)
            )}");
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.DecodePixelWidth = 50;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.UriSource = uri;
            bitmapImage.EndInit();
            bitmapImage.Freeze(); 
            return bitmapImage;
        }
        catch (KeyNotFoundException ex)
        {
            return null;
        }

        return null;
    }

    public object[] ConvertBack(object? value, Type[] targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}