using System.Reactive;
using Avalonia.Controls;
using Avalonia.Controls.Chrome;
using FluentAvalonia.UI.Controls;
using FluentAvalonia.UI.Windowing;
using ValoCord.ViewModels;

namespace ValoCord.Views
{
    public partial class MainWindow : AppWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            TitleBar.ExtendsContentIntoTitleBar = true;
        }
    }
}