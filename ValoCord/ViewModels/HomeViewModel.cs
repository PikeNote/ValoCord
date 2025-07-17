using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using ValoCord.Data;
using ValoCord.Handlers;

namespace ValoCord.ViewModels;

public partial class HomeViewModel : ViewModelBase
{
    public ObservableCollection<NewsData> RecentPatches { get; set; } = new ObservableCollection<NewsData>();
    
    [ObservableProperty]
    private Visibility _isLoading;
    public ObservableCollection<VodListItemViewModel> RecentVodCollection { get; set; } = new ObservableCollection<VodListItemViewModel>();


    public HomeViewModel()
    {
        var patches = ValorantPatchNotes.FetchLatestPatch();
        foreach (var patch in patches)
        {
            RecentPatches.Add(patch);
        }
        _ = LoadVodsAsync();
    }

    private async Task LoadVodsAsync()
    {
        IsLoading = Visibility.Visible;
        RecentVodCollection.Clear();
        
        try 
        {
            var loadedData = await Task.Run(() => DatabaseHandler.GetRecentGames());
            
            foreach (var gameData in loadedData)
            {
                RecentVodCollection.Add(new VodListItemViewModel{GameData = gameData});
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while loading VODs: {ex.Message}");
        }
        finally
        {
            IsLoading = Visibility.Collapsed;
        }
    }
}