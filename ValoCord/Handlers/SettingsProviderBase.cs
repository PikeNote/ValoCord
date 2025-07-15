using System.IO;
using Newtonsoft.Json;

namespace ValoCord.Handlers;

public class SettingsProviderBase<T>
    where T : class, new()
{
    private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings()
    {
        ObjectCreationHandling = ObjectCreationHandling.Replace
    };
    
    public T Value
    {
        get => _value;
        set => _value = value;
    }
    private T _value = new();

    public T Load(string path)
    {
        try
        {
            var fileContent = File.ReadAllText(Path.Combine(Paths.ValoCordPath, path));
            var settingsData = JsonConvert.DeserializeObject<T>(fileContent, SerializerSettings);
            if (settingsData != null)
            {
                Value = settingsData;
                return settingsData;
            }
        }
        catch (FileNotFoundException e)
        {
            Save(path);
        }
        catch (FileLoadException e)
        {

        }
        return new T();
    }

    public async Task Save(string path)
    {
        try
        {
            var settingsData = JsonConvert.SerializeObject(_value);
            await File.WriteAllTextAsync(Path.Combine(Paths.ValoCordPath, path), settingsData);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}