using System.Windows;
using System.Windows.Threading;
using Microsoft.Extensions.DependencyInjection;
using ValoCord.Handlers;
using ValoCord.ViewModels;
using ValoCord.Views;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace ValoCord;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : FluentWindow
{
    private readonly INavigationService _navigationService;
    
    public MainWindow(IServiceProvider serviceProvider, MainWindowViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;

        this.Loaded += (_, _) =>
        {
            Wpf.Ui.Appearance.ApplicationThemeManager.Apply(Wpf.Ui.Appearance.ApplicationTheme.Dark);
            Application.Current.Dispatcher.InvokeAsync(() =>
            {
                var navigationService = ServiceManager.Services.GetService<INavigationService>();

                navigationService.SetNavigationControl(RootNavigation);
                RootNavigation.SetServiceProvider(ServiceManager.Services);
                navigationService.Navigate(typeof(Home));
            }, DispatcherPriority.Loaded);
        };
            

    }
}