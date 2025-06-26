using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ReactiveUI;
using ValoCord.Data;
using ValoCord.Handlers;

namespace ValoCord.ViewModels;

public class VodsListViewModel : ViewModelBase, IActivatableViewModel
{
    public ViewModelActivator Activator { get; } = new();
    public ObservableCollection<VODListItemViewModel> RecordedVODs { get; set; } = new ObservableCollection<VODListItemViewModel>();
    
    private List<VODListItemViewModel> _completeRecordedVODs = new List<VODListItemViewModel>();

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        private set => this.RaiseAndSetIfChanged(ref _isLoading, value);
    }
    
    public VodsListViewModel()
    {
        this.WhenActivated((CompositeDisposable _) =>
        {
            LoadVodsAsync();
        });
    }
    
    
    public async Task LoadVodsAsync()
    {
        IsLoading = true;
        RecordedVODs.Clear();

        try {
            var loadedData = DatabaseHandler.GetRecentGames();
            
            foreach (var gameData in loadedData)
            {
                _completeRecordedVODs.Add(new VODListItemViewModel(gameData));
                //RecordedVODs.Add(new VODListItemViewModel(gameData));
            }
        
            //_completeRecordedVODs.Sort((x, y) => y.RecordingStartTime.CompareTo(x.RecordingStartTime));
            foreach (var vodListItemViewModel in _completeRecordedVODs)
            {
                RecordedVODs.Add(vodListItemViewModel);
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