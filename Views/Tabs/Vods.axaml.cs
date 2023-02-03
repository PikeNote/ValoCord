using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ValCord.Views.Tabs;

public partial class Vods : UserControl
{
    public Vods()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}