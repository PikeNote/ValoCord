using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ValoCord_WPF.Data;
using ValoCord_WPF.ViewModels;

namespace ValoCord_WPF.Views;

public partial class Settings : Page
{
    public Settings()
    {
        InitializeComponent();
        
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
    
    private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
    {
        Regex regex = new Regex("[^0-9]+");
        e.Handled = regex.IsMatch(e.Text);
    }
}