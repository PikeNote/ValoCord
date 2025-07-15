using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace ValoCord_WPF.Converters;

public class StringImageUrlConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string url || string.IsNullOrEmpty(url))
        {
            return null;
        }
        
        if (!int.TryParse(parameter?.ToString(), out int decodeWidth) || decodeWidth <= 0)
        {
            decodeWidth = 200; 
        }

        try
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(url, UriKind.Absolute);
            bitmap.DecodePixelWidth = decodeWidth;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            return bitmap;
        }
        catch
        {
            return null;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}