using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AsyncImageLoader;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace ValoCord.Handlers;

public class ScaledImageWebLoader : IAsyncImageLoader
{
    private readonly int _decodeWidth;
    private readonly HttpClient _httpClient = new();

    public ScaledImageWebLoader(int decodeWidth = 300)
    {
        _decodeWidth = decodeWidth;
    }

    public async Task<Bitmap> ProvideImageAsync(string url)
    {
        try
        {
            Uri uri = new(url, UriKind.RelativeOrAbsolute);
            
            if (uri.Scheme == "http" || uri.Scheme == "https")
            {
                var data = await _httpClient.GetByteArrayAsync(uri);
                using var stream = new MemoryStream(data);
                return Bitmap.DecodeToWidth(stream, _decodeWidth);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load image: {ex.Message}");
        }

        return null;
    }
    
    public void Dispose()
    {
        _httpClient.Dispose();
    }
}