using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using ValoCord.Data;

namespace ValoCord.Converters;

public class WeaponIconNameConverter : IMultiValueConverter
{
    public object? Convert(IList<object>? values, Type targetType, object? parameter, CultureInfo culture)
    {
        if(values[0] is not string weaponUUID || string.IsNullOrEmpty(weaponUUID) || values[2] is not Dictionary<String, PlayerData> players || values[1] is not string killerUUID) {
            return null;
        }

        weaponUUID = weaponUUID.ToLower().Trim();
        try
        {
            if (weaponUUID == "ultimate")
            {
                var userAgent = AgentData.GetAgentNames(players[killerUUID].character_played);
                var uri = new Uri($"avares://Valocord{AgentData.GetUltimateName(userAgent)}");
                return new Bitmap(AssetLoader.Open(uri));
            }
            else
            {
                var uri = new Uri($"avares://Valocord{WeaponData.GetFileName(WeaponData.GetDisplayName(weaponUUID))}");
                return new Bitmap(AssetLoader.Open(uri));
            }
        }
        catch (Exception)
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
