using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;

namespace ValoCord.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ViewModelBase _currentPage;
        private NavigationItem _selectedMenuItem;

        public ViewModelBase CurrentPage
        {
            get => _currentPage;
            set => this.RaiseAndSetIfChanged(ref _currentPage, value);
        }

        public NavigationItem SelectedMenuItem
        {
            get => _selectedMenuItem;
            set => this.RaiseAndSetIfChanged(ref _selectedMenuItem, value);
        }
        
        public ReactiveCommand<Unit, Unit> NavigateToSettingsCommand { get; }


        public ObservableCollection<NavigationItem> MenuItems { get; }

        public MainWindowViewModel()
        {
            MenuItems = new ObservableCollection<NavigationItem>
            {
                new NavigationItem("Home", "Home", new HomeViewModel()),
                new NavigationItem("VODs", "Video", new VodsListViewModel()),
                new NavigationItem("Clips", "SlideShow", new ClipsViewModel())
            };

            this.WhenAnyValue(vm => vm.SelectedMenuItem)
                .Where(item => item != null)
                .Subscribe(selectedItem => CurrentPage = selectedItem.ViewModel);
            SelectedMenuItem = MenuItems[0];
        }

        public void NavigateToSettings()
        {
            CurrentPage = new SettingsViewModel();
        }
    }
    
    
    public class NavigationItem
    {
        public string Content { get; }
        public string IconSource { get; }
        public ViewModelBase ViewModel { get; }

        public NavigationItem(string content, string iconSource, ViewModelBase viewModel)
        {
            Content = content;
            IconSource = iconSource;
            ViewModel = viewModel;
        }
    }
}