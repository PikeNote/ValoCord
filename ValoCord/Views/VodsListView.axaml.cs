using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ValoCord.ViewModels;

namespace ValoCord.Views;

public partial class VodsListView : ReactiveUserControl<VodsListViewModel>
{
    public VodsListView()
    {
        InitializeComponent();
    }
}