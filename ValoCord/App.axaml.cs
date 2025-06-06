using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Logging;
using Avalonia.Markup.Xaml;
using ValoCord.Handlers;
using ValoCord.Video;
using ValoCord.ViewModels;
using ValoCord.Views;

namespace ValoCord
{
    public partial class App : Application
    {
        public static VLCPlayerService AppNativeVideoPlayerService = new VLCPlayerService();
        
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
            ProcessHandler.Initialize();
            Logs.Initialize();
            
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}