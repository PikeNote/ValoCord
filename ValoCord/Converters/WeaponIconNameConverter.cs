using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using ValoCord.Data;

namespace ValoCord.Converters;

public class WeaponIconNameConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string weaponUUID && !string.IsNullOrEmpty(weaponUUID))
        {
            weaponUUID = weaponUUID.ToLower().Trim();
            Console.WriteLine(weaponUUID + ".v.");
            Console.WriteLine(WeaponData.GetFileName("Bulldog"));
            try
            {
                var uri = new Uri($"avares://Valocord{WeaponData.GetFileName(WeaponData.GetDisplayName(weaponUUID))}");
                Console.WriteLine(uri);
                return new Bitmap(AssetLoader.Open(uri));
            }
            catch (Exception)
            {
                return null;
            }
        }
        else
        {
            Console.WriteLine("Test");
        }
        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
