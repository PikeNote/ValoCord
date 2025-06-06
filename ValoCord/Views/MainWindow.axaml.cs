using Avalonia.Controls;
using Avalonia.Controls.Chrome;
using FluentAvalonia.UI.Controls;
using FluentAvalonia.UI.Windowing;

namespace ValoCord.Views
{
    public partial class MainWindow : AppWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            TitleBar.ExtendsContentIntoTitleBar = true;
            
            var window = new VODViewer();
            window.Show();
        }

        private void NavView_ItemInvoked(
            object? sender,
            NavigationViewItemInvokedEventArgs e)
        {
            // hide them all
            HomeView.IsVisible = false;
            VodsView.IsVisible = false;
            ClipsView.IsVisible = false;

            // show the one that was clicked
            var tag = (e.InvokedItemContainer as NavigationViewItem)?.Tag as string;
            switch (tag)
            {
                case "Home":  HomeView.IsVisible = true; break;
                case "Vods":  VodsView.IsVisible = true; break;
                case "Clips": ClipsView.IsVisible = true; break;
            }
        }
    }
}