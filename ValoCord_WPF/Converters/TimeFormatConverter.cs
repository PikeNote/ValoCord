using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace ValoCord_WPF.Converters;

public class TimeFormatConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // The URL comes from the main binding 'value'
        if (value is not double time)
        {
            return null; // Or a placeholder
        }
        
        TimeSpan timeSpan = TimeSpan.FromMilliseconds(time);
        
        if (timeSpan.TotalHours >= 1)
        {
            return timeSpan.ToString(@"hh\:mm\:ss");
        }
        else
        {
            return timeSpan.ToString(@"mm\:ss");
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}