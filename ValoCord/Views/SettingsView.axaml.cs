using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ValoCord.Data;
using ValoCord.ViewModels;

namespace ValoCord.Views;

public partial class SettingsView : UserControl
{
    public SettingsView()
    {
        InitializeComponent();
        InputVolume.AddHandler(PointerReleasedEvent, InputElement_OnPointerReleased, RoutingStrategies.Tunnel);
        OutputVolume.AddHandler(PointerReleasedEvent, InputElement_OnPointerReleased, RoutingStrategies.Tunnel);
        BitrateBox.AddHandler(KeyDownEvent, InputElement_OnKeyDown, RoutingStrategies.Tunnel);
    }

    private void InputElement_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (DataContext is SettingsViewModel ViewModel)
        {
            ViewModel.SaveSettings();
        }
    }

    private void InputElement_OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            var parent = (sender as Control)?.Parent as IInputElement;
            parent?.Focus();
            
            e.Handled = true;
        }
    }

    private void ToggleButton_OnIsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        var checkBox = sender as CheckBox;
        if (checkBox == null) return;
        
        if (DataContext is SettingsViewModel ViewModel)
        {
            bool newValue = checkBox.IsChecked ?? false;
            
            var dataItem = checkBox.DataContext as GameModeSettings;
            if (dataItem == null) return;

            dataItem.Enabled = newValue;

            ViewModel.SaveSettings();
        }
    }
}