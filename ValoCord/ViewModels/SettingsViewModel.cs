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
    private SettingsProviderBase<SettingsData> _settingsProvider;
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
            _settingsProvider.Value.SelectedInputDeviceName.DeviceName = SelectedInputDevice.FriendlyName;
            OnPropertyChanged(nameof(SelectedInputDevice));
        }
    }
    
    public AudioDevice SelectedOutputDevice
    {
        get => _selectedOutputDevice;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedOutputDevice, value);
            _settingsProvider.Value.SelectedOutputDeviceName.DeviceName = SelectedOutputDevice.FriendlyName;
            OnPropertyChanged(nameof(SelectedOutputDevice));
        }
    }

    public int InputAudioVolume
    {
        get => _settingsProvider.Value.SelectedInputDeviceName.Volume;
        set
        {
            _settingsProvider.Value.SelectedInputDeviceName.Volume = value;
            OnPropertyChanged(nameof(InputAudioVolume));
        }
    }
    
    public int OutputAudioVolume
    {
        get => _settingsProvider.Value.SelectedOutputDeviceName.Volume;
        set
        {
            _settingsProvider.Value.SelectedOutputDeviceName.Volume = value; 
            OnPropertyChanged(nameof(OutputAudioVolume));
        }
    }
    
    public int BitrateValue
    {
        get => _settingsProvider.Value.Bitrate;
        set => _settingsProvider.Value.Bitrate = value;
    }
    
    public int FramesPerSecondValue
    {
        get => _settingsProvider.Value.FrameRate;
        set => _settingsProvider.Value.FrameRate = value;
    }

    public Boolean IsVBR => _settingsProvider.Value.EncoderBitRateMode == BitrateControlMode.VBR;
    public int Quality => _settingsProvider.Value.Quality;
    public Array VideoEncoderOptions => Enum.GetValues(typeof(VideoEncoderFormat));
    public Array BitRateControlOptions => Enum.GetValues(typeof(BitrateControlMode));

    public VideoEncoderFormat SelectedVideoEncoderFormat
    {
        get => _settingsProvider.Value.Encoder;
        set
        {
            _settingsProvider.Value.Encoder = value;
            OnPropertyChanged(nameof(SelectedVideoEncoderFormat));
        }
    }
    
    public BitrateControlMode SelectedBitRateControl
    {
        get => _settingsProvider.Value.EncoderBitRateMode;
        set
        {
            _settingsProvider.Value.EncoderBitRateMode = value;
            OnPropertyChanged(nameof(SelectedBitRateControl));
            OnPropertyChanged(nameof(IsVBR));
        }
    }

    public Boolean HardwareAccelerationEnabled
    {
        get => _settingsProvider.Value.HardwareAcceleration;
        set => _settingsProvider.Value.HardwareAcceleration = value;
    }
    
    public Boolean ThrottlingEnabled
    {
        get => _settingsProvider.Value.ThrottlingEnabled;
        set => _settingsProvider.Value.ThrottlingEnabled = value;
    }

    public Boolean LowLatencyEnabled
    {
        get => _settingsProvider.Value.IsLowLatencyEnabled;
        set => _settingsProvider.Value.IsLowLatencyEnabled = value;
    }
    
    public List<GameModeSettings> GameModeEnabled
    {
        get => _settingsProvider.Value.EnabledGameModes;
        set => _settingsProvider.Value.EnabledGameModes = value;
    }
    
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    public SettingsViewModel()
    {
        _settingsProvider = new SettingsProviderBase<SettingsData>();
        _settingsProvider.Load("settings.json");
        
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
        
        SelectedInputDevice = InputAudioDevices.FirstOrDefault(d => d.FriendlyName == _settingsProvider.Value.SelectedInputDeviceName.DeviceName)
                              ?? InputAudioDevices.First();
        SelectedOutputDevice = OutputAudioDevices.FirstOrDefault(d => d.FriendlyName == _settingsProvider.Value.SelectedOutputDeviceName.DeviceName)
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
        _settingsProvider.Save("settings.json");
    }
}