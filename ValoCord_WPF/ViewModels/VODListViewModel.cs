using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using ReactiveUI;
using ValoCord_WPF.Handlers;

namespace ValoCord_WPF.ViewModels;

public partial class VODListViewModel : ViewModelBase
{
    public ViewModelActivator Activator { get; } = new();
    public ObservableCollection<VODListItemViewModel> RecordedVODs { get; set; } = new ObservableCollection<VODListItemViewModel>();

    [ObservableProperty]
    private Visibility _isLoading;
    
    public VODListViewModel()
    {
        LoadVodsAsync();
    }
    
    
    public async Task LoadVodsAsync()
    {
        IsLoading = Visibility.Visible;
        RecordedVODs.Clear();

        try {
            var loadedData = DatabaseHandler.GetRecentGames();
            foreach (var gameData in loadedData)
            {
                RecordedVODs.Add(new VODListItemViewModel{_gameData = gameData});
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