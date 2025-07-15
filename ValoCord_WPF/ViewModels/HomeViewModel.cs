using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using ValoCord_WPF.Data;
using ValoCord_WPF.Handlers;

namespace ValoCord_WPF.ViewModels;

public partial class HomeViewModel : ViewModelBase
{
    public List<NewsData> RecentPatches => ValorantPatchNotes.FetchLatestPatch();
    
    [ObservableProperty]
    private Visibility _isLoading;
    public ObservableCollection<VODListItemViewModel> RecentVODCollection { get; set; } = new ObservableCollection<VODListItemViewModel>();


    public HomeViewModel()
    {
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