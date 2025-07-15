using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;

using ValoCord_WPF.Handlers;
using ValoCord_WPF.ViewModels;
using ValoCord_WPF.Views;
using Wpf.Ui;
using Wpf.Ui.Appearance;
using Wpf.Ui.DependencyInjection;

namespace ValoCord_WPF;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public App()
    {
        ProcessHandler.Initialize();
        Logs.Initialize();
        DatabaseHandler.Initialize();
        LibVLCLoader.Initialize();
        ApplicationSettings.Initialize();
        
        var services = new ServiceCollection();

        services.AddSingleton<INavigationService, NavigationService>();
        
        services.AddNavigationViewPageProvider();
        
        services.AddTransient<MainWindow>();
        services.AddTransient<MainWindowViewModel>();
        
        services.AddTransient<Home>();
        services.AddTransient<HomeViewModel>();

        services.AddTransient<VODListView>();
        services.AddTransient<VODListViewModel>();
        
        services.AddTransient<Settings>();
        services.AddTransient<SettingsViewModel>();

        ServiceManager.Services= services.BuildServiceProvider();
        
        var mainWindow = ServiceManager.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();
        
        ApplicationThemeManager.Apply(ApplicationTheme.Dark);
        
        Unosquare.FFME.Library.FFmpegDirectory = @"C:\ffmpeg";
    }
}