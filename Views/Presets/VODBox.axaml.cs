using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ValCord.Views.Presets;

public partial class VODBox : UserControl
{
    public VODBox()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}