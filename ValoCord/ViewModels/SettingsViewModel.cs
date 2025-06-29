using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using ScreenRecorderLib;
using ValoCord.Data;
using ValoCord.Handlers;
using AudioDevice = ScreenRecorderLib.AudioDevice;

namespace ValoCord.ViewModels;

public class SettingsViewModel : ViewModelBase, INotifyPropertyChanged
{
    private AudioDevice _selectedInputDevice;
    private AudioDevice _selectedOutputDevice;
    
    private List<AudioDevice> InputAudioDevices  { get; set; }
    private List<AudioDevice> OutputAudioDevices { get; set; }
    
    public event PropertyChangedEventHandler PropertyChanged;
    
    public AudioDevice SelectedInputDevice
    {
        get => _selectedInputDevice;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedInputDevice, value);
            ApplicationSettings.SettingsData.Value.SelectedInputDeviceName.DeviceName = SelectedInputDevice.FriendlyName;
            OnPropertyChanged(nameof(SelectedInputDevice));
        }
    }
    
    public AudioDevice SelectedOutputDevice
    {
        get => _selectedOutputDevice;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedOutputDevice, value);
            ApplicationSettings.SettingsData.Value.SelectedOutputDeviceName.DeviceName = SelectedOutputDevice.FriendlyName;
            OnPropertyChanged(nameof(SelectedOutputDevice));
        }
    }

    public int InputAudioVolume
    {
        get => ApplicationSettings.SettingsData.Value.SelectedInputDeviceName.Volume;
        set
        {
            ApplicationSettings.SettingsData.Value.SelectedInputDeviceName.Volume = value;
            OnPropertyChanged(nameof(InputAudioVolume));
        }
    }
    
    public int OutputAudioVolume
    {
        get => ApplicationSettings.SettingsData.Value.SelectedOutputDeviceName.Volume;
        set
        {
            ApplicationSettings.SettingsData.Value.SelectedOutputDeviceName.Volume = value; 
            OnPropertyChanged(nameof(OutputAudioVolume));
        }
    }
    
    public int BitrateValue
    {
        get => ApplicationSettings.SettingsData.Value.Bitrate;
        set => ApplicationSettings.SettingsData.Value.Bitrate = value;
    }
    
    public int FramesPerSecondValue
    {
        get => ApplicationSettings.SettingsData.Value.FrameRate;
        set => ApplicationSettings.SettingsData.Value.FrameRate = value;
    }

    public Boolean IsVBR => ApplicationSettings.SettingsData.Value.EncoderBitRateMode == BitrateControlMode.VBR;
    public int Quality => ApplicationSettings.SettingsData.Value.Quality;
    public Array VideoEncoderOptions => Enum.GetValues(typeof(VideoEncoderFormat));
    public Array BitRateControlOptions => Enum.GetValues(typeof(BitrateControlMode));

    public VideoEncoderFormat SelectedVideoEncoderFormat
    {
        get => ApplicationSettings.SettingsData.Value.Encoder;
        set
        {
            ApplicationSettings.SettingsData.Value.Encoder = value;
            OnPropertyChanged(nameof(SelectedVideoEncoderFormat));
        }
    }
    
    public BitrateControlMode SelectedBitRateControl
    {
        get => ApplicationSettings.SettingsData.Value.EncoderBitRateMode;
        set
        {
            ApplicationSettings.SettingsData.Value.EncoderBitRateMode = value;
            OnPropertyChanged(nameof(SelectedBitRateControl));
            OnPropertyChanged(nameof(IsVBR));
        }
    }

    public Boolean HardwareAccelerationEnabled
    {
        get => ApplicationSettings.SettingsData.Value.HardwareAcceleration;
        set => ApplicationSettings.SettingsData.Value.HardwareAcceleration = value;
    }
    
    public Boolean ThrottlingEnabled
    {
        get => ApplicationSettings.SettingsData.Value.ThrottlingEnabled;
        set => ApplicationSettings.SettingsData.Value.ThrottlingEnabled = value;
    }

    public Boolean LowLatencyEnabled
    {
        get => ApplicationSettings.SettingsData.Value.IsLowLatencyEnabled;
        set => ApplicationSettings.SettingsData.Value.IsLowLatencyEnabled = value;
    }
    
    public List<GameModeSettings> GameModeEnabled
    {
        get => ApplicationSettings.SettingsData.Value.EnabledGameModes;
        set => ApplicationSettings.SettingsData.Value.EnabledGameModes = value;
    }
    
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    public SettingsViewModel()
    {
        var inputDevices = new List<AudioDevice>
        {
            new AudioDevice { FriendlyName = "System Default" }
        };
        inputDevices.AddRange(Recorder.GetSystemAudioDevices(AudioDeviceSource.InputDevices));
        InputAudioDevices = inputDevices;
        
        var outputDevices = new List<AudioDevice>
        {
            new AudioDevice { FriendlyName = "System Default" }
        };
        outputDevices.AddRange(Recorder.GetSystemAudioDevices(AudioDeviceSource.OutputDevices));
        OutputAudioDevices = outputDevices;
        
        SelectedInputDevice = InputAudioDevices.FirstOrDefault(d => d.FriendlyName == ApplicationSettings.SettingsData.Value.SelectedInputDeviceName.DeviceName)
                              ?? InputAudioDevices.First();
        SelectedOutputDevice = OutputAudioDevices.FirstOrDefault(d => d.FriendlyName == ApplicationSettings.SettingsData.Value.SelectedOutputDeviceName.DeviceName)
                               ?? OutputAudioDevices.First();
        
        var anyCheckChanged = this.GameModeEnabled
            .Select(item => item.WhenAnyValue(x => x.Enabled))
            .Merge();
        
        var settingsChanged = Observable.Merge(
            this.WhenAnyValue(x => x.SelectedInputDevice).Select(_ => Unit.Default),
            this.WhenAnyValue(x => x.SelectedOutputDevice).Select(_ => Unit.Default),
            this.WhenAnyValue(x => x.SelectedVideoEncoderFormat).Select(_ => Unit.Default),
            this.WhenAnyValue(x=>x.ThrottlingEnabled).Select(_ => Unit.Default),
            this.WhenAnyValue(x=>x.HardwareAccelerationEnabled).Select(_ => Unit.Default),
            this.WhenAnyValue(x=>x.LowLatencyEnabled).Select(_ => Unit.Default),
            anyCheckChanged.Select(_ => Unit.Default)
        );
        
        settingsChanged
            .Throttle(TimeSpan.FromMilliseconds(500))
            .Skip(1) 
            .ObserveOn(RxApp.MainThreadScheduler) 
            .Subscribe(_ => SaveSettings());
    }

    public void SaveSettings()
    {
        ApplicationSettings.SettingsData.Save("settings.json");
    }
}