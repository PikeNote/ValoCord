using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ValCord.ViewModels;

namespace ValCord.Views.Tabs;

public partial class Home : UserControl
{
    public Home()
    {
        InitializeComponent();
        DataContext = new HomeViewModel();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}