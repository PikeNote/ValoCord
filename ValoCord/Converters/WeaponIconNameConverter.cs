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
        string? weaponUUID = values[0] as string ?? "";
        if(values[2] is not Dictionary<String, PlayerData> players || values[1] is not string killerUUID) {
            return null;
        }

        weaponUUID = weaponUUID.ToLower().Trim();
        try
        {
            Uri uri;

            switch (weaponUUID)
            {
                case "ultimate":
                    uri = new Uri($"avares://Valocord{AgentData.GetUltimateImage(AgentData.GetAgentNames(players[killerUUID].character_played))}");
                    break;
                case "ability1":
                    uri = new Uri($"avares://Valocord{AgentData.GetAbility1Image(AgentData.GetAgentNames(players[killerUUID].character_played))}");
                    break;
                case "ability2":
                    uri = new Uri($"avares://Valocord{AgentData.GetAbility2Image(AgentData.GetAgentNames(players[killerUUID].character_played))}");
                    break;
                case "grenadeability":
                    uri = new Uri($"avares://Valocord{AgentData.GetGrenadeImage(AgentData.GetAgentNames(players[killerUUID].character_played))}");
                    break;
                case "":
                    uri = new Uri($"avares://Valocord{WeaponData.GetFileName("Melee")}");
                    break;
                default:
                    uri = new Uri($"avares://Valocord{WeaponData.GetFileName(WeaponData.GetDisplayName(weaponUUID))}");
                    break;
            }
            return new Bitmap(AssetLoader.Open(uri));
        }
        catch (Exception)
        {
            return new Uri("avares://Valocord/Assets/Default/AssetNotFound.png");
        }
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
