using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using ValoCord.Views;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace ValoCord.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<object> _navigationItems = [];
    
    [ObservableProperty]
    private ObservableCollection<object> _footerNavigationItems = [];
    
    private readonly INavigationService _navigationService;

    public ICommand NavigateToSettingsCommand { get; }

    public MainWindowViewModel(INavigationService navigationService)
    {
        
        _navigationService = navigationService;
        
        NavigationItems =
        [
            new NavigationViewItem()
            {
                Content = "Home",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Home24 },
                TargetPageType = typeof(Home),
                
            },
            new NavigationViewItem()
            {
                Content = "VODs",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Video24 },
                TargetPageType = typeof(VODListView),
                
            },
        ];
        
        FooterNavigationItems = 
        [
            new NavigationViewItem()
            {
                Content = "Settings",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Settings24 },
                TargetPageType = typeof(Settings)
            }
        ];
    }

}