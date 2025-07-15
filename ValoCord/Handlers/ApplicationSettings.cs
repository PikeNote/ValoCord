using ValoCord.Data;

namespace ValoCord.Handlers;

public static class ApplicationSettings
{
    public static SettingsProviderBase<SettingsData> SettingsData = new();

    public static void Initialize()
    {
        SettingsData.Load("settings.json");
    }
}