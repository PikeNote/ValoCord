using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using ReactiveUI;
using ValoCord.Handlers;

namespace ValoCord.ViewModels;

public partial class VodListViewModel : ViewModelBase
{
    public ViewModelActivator Activator { get; } = new();
    public ObservableCollection<VodListItemViewModel> RecordedVODs { get; set; } = new ObservableCollection<VodListItemViewModel>();

    [ObservableProperty]
    private Visibility _isLoading;
    
    public VodListViewModel()
    {
        _ = LoadVodsAsync();
    }


    private async Task LoadVodsAsync()
    {
        await Task.Run(() =>
        {
            IsLoading = Visibility.Visible;
            RecordedVODs.Clear();

            try
            {
                var loadedData = DatabaseHandler.GetRecentGames();
                foreach (var gameData in loadedData)
                {
                    RecordedVODs.Add(new VodListItemViewModel { GameData = gameData });
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
        });
    }
}