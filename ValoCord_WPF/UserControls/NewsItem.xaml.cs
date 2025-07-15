using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ValoCord_WPF.Data;

namespace ValoCord_WPF.UserControls;

public partial class NewsItem : UserControl
{
    public NewsItem()
    {
        InitializeComponent();
        this.DataContextChanged += NewsItem_DataContextChanged;
    }
    
    private void NewsItem_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (this.DataContext is NewsData nd && nd.media != null && !string.IsNullOrEmpty(nd.media.url))
        {

                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(nd.media.url, UriKind.Absolute);
                bitmap.DecodePixelWidth = 500;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                ImageBorder.Background = new ImageBrush
                {
                    ImageSource = bitmap,
                    Stretch = Stretch.UniformToFill
                };
            
        }
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        if (DataContext is NewsData nd)
        {
            var url = nd.action.payload.url;
            try
            {
                Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
            }
            catch
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}