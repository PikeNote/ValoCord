using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ValCord.Views.Tabs;

public partial class Clips : UserControl
{
    public Clips()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}