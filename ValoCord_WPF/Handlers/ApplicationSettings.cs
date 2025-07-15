using ValoCord_WPF.Data;
using ValoCord_WPF.Data;

namespace ValoCord_WPF.Handlers;

public static class ApplicationSettings
{
    public static SettingsProviderBase<SettingsData> SettingsData = new();

    public static void Initialize()
    {
        SettingsData.Load("settings.json");
    }
}