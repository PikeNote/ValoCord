using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace ValoCord.Converters;

public class StandingToColorConverter : IValueConverter
{
    private Color _firstStanding = Color.Parse("#C3C750");
    private Color _secondStanding = Color.Parse("#C4C4C4");
    private Color _thirdStanding = Color.Parse("#CE8946");
    private Color _fourthStanding = Color.Parse("#424242");

    public object? Convert(object value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            "1st" => _firstStanding,
            "2nd" => _secondStanding,
            "3rd" => _thirdStanding,
            _ => _fourthStanding
        };
    }
    
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}