using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using ValoCord.Data;
using ValoCord.Handlers;

namespace ValoCord.ViewModels;

public class VodsViewViewmodel : ViewModelBase
{
    public ObservableCollection<VODListItemViewModel> RecordedVODs { get; } = new ObservableCollection<VODListItemViewModel>();

    public VodsViewViewmodel()
    {
        string [] fileEntries = Directory.GetFiles(Paths.DefaultDataPath);
        Console.WriteLine(string.Join(", ", fileEntries));
        foreach (string fileName in fileEntries)
        {
            var gameData = JsonConvert.DeserializeObject<GameData>(File.ReadAllText(fileName));
            if (gameData != null)
            {
                RecordedVODs.Add(new VODListItemViewModel(gameData));
                Console.WriteLine("Added " + fileName);
            }
        }
    }
}