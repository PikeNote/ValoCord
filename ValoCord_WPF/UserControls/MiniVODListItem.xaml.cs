using System.Windows;
using System.Windows.Controls;
using ValoCord_WPF.ViewModels;
using ValoCord_WPF.Views;

namespace ValoCord_WPF.UserControls;

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