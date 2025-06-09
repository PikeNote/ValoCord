using System;
using System.IO;

namespace ValoCord.Handlers;

public static class Paths
{
    public static string ValoCordPath = Path.Combine(Environment.GetFolderPath(
        Environment.SpecialFolder.LocalApplicationData), "ValoCord");
    public static readonly string RecordingLogPath = Path.Combine(Environment.GetFolderPath(
        Environment.SpecialFolder.LocalApplicationData), "ValoCord\\recorder.log");
    public static readonly string DefaultVideoPath = Path.Combine(Environment.GetFolderPath(
        Environment.SpecialFolder.LocalApplicationData), "ValoCord\\videos");
    public static readonly string DefaultDataPath = Path.Combine(Environment.GetFolderPath(
        Environment.SpecialFolder.LocalApplicationData), "ValoCord\\data");

    public static String generateVideoPath(string matchID)
    {
        return Path.Combine(DefaultVideoPath, matchID + ".mp4");
    }
}