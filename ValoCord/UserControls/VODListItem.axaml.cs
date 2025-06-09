using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ValoCord.ViewModels;
using ValoCord.Views;

namespace ValoCord.UserControls;

public partial class VODListItem : UserControl
{
    public VODListItem()
    {
        InitializeComponent();
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is VODListItemViewModel viewModel)
        {
            var vodWindow = new VODViewer(viewModel.GetGameData());
            vodWindow.Show();
        }
    }
}