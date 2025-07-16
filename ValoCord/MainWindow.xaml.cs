using System.Windows;
using System.Windows.Threading;
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
    public MainWindow(IServiceProvider serviceProvider, MainWindowViewModel viewModel, INavigationService navigationService)
    {
        InitializeComponent();
        DataContext = viewModel;

        Loaded += (_, _) =>
        {
            Wpf.Ui.Appearance.ApplicationThemeManager.Apply(Wpf.Ui.Appearance.ApplicationTheme.Dark);
            Application.Current.Dispatcher.InvokeAsync(() =>
            {
                navigationService.SetNavigationControl(RootNavigation);
                if (ServiceManager.Services != null) RootNavigation.SetServiceProvider(ServiceManager.Services);
                navigationService.Navigate(typeof(Home));
            }, DispatcherPriority.Loaded);
        };
            

    }
}