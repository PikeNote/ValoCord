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
    public ObservableCollection<VODListItemViewModel> RecentVODCollection { get; set; } = new ObservableCollection<VODListItemViewModel>();


    public HomeViewModel()
    {
        var patches = ValorantPatchNotes.FetchLatestPatch();
        foreach (var patch in patches)
        {
            RecentPatches.Add(patch);
        }
        LoadVodsAsync();
    }
    
    public async Task LoadVodsAsync()
    {
        IsLoading = Visibility.Visible;
        RecentVODCollection.Clear();

        try {
            var loadedData = DatabaseHandler.GetRecentGames();
            foreach (var gameData in loadedData)
            {
                RecentVODCollection.Add(new VODListItemViewModel{_gameData = gameData});
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