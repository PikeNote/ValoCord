using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ValCord.Views.Tabs;

public partial class Settings : UserControl
{
    public Settings()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}