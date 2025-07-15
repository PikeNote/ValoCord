using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Extensions.DependencyInjection;
using ValoCord_WPF.Handlers;
using ValoCord_WPF.ViewModels;
using ValoCord_WPF.Views;
using Wpf.Ui;
using Wpf.Ui.Abstractions;
using Wpf.Ui.Controls;

namespace ValoCord_WPF;

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