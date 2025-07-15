using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ValoCord.Converters;

public class StandingToColorConverter : IValueConverter
{
    private static readonly SolidColorBrush _firstStanding = new ((Color)ColorConverter.ConvertFromString("#C3C750")!);
    private static readonly SolidColorBrush _secondStanding = new ((Color)ColorConverter.ConvertFromString("#C4C4C4")!);
    private static readonly SolidColorBrush _thirdStanding = new((Color)ColorConverter.ConvertFromString("#CE8946")!);
    private static readonly SolidColorBrush _fourthStanding = new((Color)ColorConverter.ConvertFromString("#424242")!);

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value switch
        {
            "1st" => _firstStanding,
            "2nd" => _secondStanding,
            "3rd" => _thirdStanding,
            _ => _fourthStanding
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}