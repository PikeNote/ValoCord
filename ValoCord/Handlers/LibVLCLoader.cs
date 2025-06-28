using System.Threading.Tasks;
using LibVLCSharp.Shared;

namespace ValoCord.Handlers;

public static class LibVLCLoader
{
    public static LibVLC LibVLC;

    public static Task Initialize()
    {
        return Task.Run(() =>
        {
            LibVLC = new LibVLC();
        });
    }
}