using System.Windows;
using System.Windows.Controls;
using ValoCord.ViewModels;
using ValoCord.Views;

namespace ValoCord.UserControls;

public partial class MiniVODListItem : UserControl
{
    public MiniVODListItem()
    {
        InitializeComponent();
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        if (DataContext is VODListItemViewModel viewModel)
        {
            var vodWindow = new VODViewer(viewModel.GetGameData());
            vodWindow.Show();
        }
    }
}