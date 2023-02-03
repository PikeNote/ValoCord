using System.Linq;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using NAudio.CoreAudioApi;

namespace ValCord.Views.Settings;

public partial class Audio : UserControl
{
    public Audio()
    {
        InitializeComponent();

        AvaloniaList<MMDevice> ad = new AvaloniaList<MMDevice>();
        var enumerator = new MMDeviceEnumerator();
        foreach (var endpoint in 
                 enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active))
        {
            ad.Add(endpoint);
        }
        
        var audioComboBox = this.Find<ComboBox>("audioComboBox");
        audioComboBox.Items = ad.Select(x => x.DeviceFriendlyName);
        audioComboBox.SelectedIndex = 0;
        
        
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}