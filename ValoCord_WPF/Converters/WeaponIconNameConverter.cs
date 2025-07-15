using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using ValoCord_WPF.Data;

namespace ValoCord_WPF.Converters;

public class WeaponIconNameConverter : IMultiValueConverter
{
    public object? Convert(object[] values, Type targetType, object? parameter, CultureInfo culture)
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
                    uri = new Uri($"pack://application:,,,{AgentData.GetUltimateImage(AgentData.GetAgentNames(players[killerUUID].character_played))}");
                    break;
                case "ability1":
                    uri = new Uri($"pack://application:,,,{AgentData.GetAbility1Image(AgentData.GetAgentNames(players[killerUUID].character_played))}");
                    break;
                case "ability2":
                    uri = new Uri($"pack://application:,,,{AgentData.GetAbility2Image(AgentData.GetAgentNames(players[killerUUID].character_played))}");
                    break;
                case "grenadeability":
                    uri = new Uri($"pack://application:,,,{AgentData.GetGrenadeImage(AgentData.GetAgentNames(players[killerUUID].character_played))}");
                    break;
                case "":
                    uri = new Uri($"pack://application:,,,{WeaponData.GetFileName("Melee")}");
                    break;
                default:
                    uri = new Uri($"pack://application:,,,{WeaponData.GetFileName(WeaponData.GetDisplayName(weaponUUID))}");
                    break;
            }

            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.DecodePixelWidth = 200;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.UriSource = uri;
            bitmapImage.EndInit();
            bitmapImage.Freeze(); 
            
            return bitmapImage;
        }
        catch (Exception)
        {
            return new Uri("pack://application:,,,/Assets/Default/AssetNotFound.png");
        }
    }

    public object[] ConvertBack(object? value, Type[] targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
