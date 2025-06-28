using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using ReactiveUI;
using ValoCord.Data;
using ValoCord.Handlers;

namespace ValoCord.ViewModels;

public class HomeViewModel : ViewModelBase, IActivatableViewModel
{
    public ViewModelActivator Activator { get; } = new();
    public List<NewsData> RecentPatches => ValorantPatchNotes.FetchLatestPatch();
    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        private set => this.RaiseAndSetIfChanged(ref _isLoading, value);
    }

    public ObservableCollection<VODListItemViewModel> RecentVODCollection { get; set; } = new ObservableCollection<VODListItemViewModel>();


    public HomeViewModel()
    {
        this.WhenActivated((CompositeDisposable _) =>
        {
            LoadVodsAsync();
        });
    }
    
    public async Task LoadVodsAsync()
    {
        //IsLoading = true;
        RecentVODCollection.Clear();

        try {
            var loadedData = DatabaseHandler.GetRecentGames();
            foreach (var gameData in loadedData)
            {
                RecentVODCollection.Add(new VODListItemViewModel(gameData));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while loading VODs: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }
}