using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using ReactiveUI;
using ScreenRecorderLib;
using ValoCord.Data;
using ValoCord.Handlers;
using AudioDevice = ScreenRecorderLib.AudioDevice;

namespace ValoCord.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    
    public List<AudioDevice> InputAudioDevices { get; }
    public List<AudioDevice> OutputAudioDevices { get; }
    
    [ObservableProperty]
    private AudioDevice _selectedInputDevice;

    [ObservableProperty]
    private AudioDevice _selectedOutputDevice;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsVBR))]
    private BitrateControlMode _selectedBitRateControl;

    [ObservableProperty]
    private VideoEncoderFormat _selectedVideoEncoderFormat;

    [ObservableProperty]
    private int _inputAudioVolume;

    [ObservableProperty]
    private int _outputAudioVolume;

    [ObservableProperty]
    private int _bitrateValue;
    
    [ObservableProperty]
    private int _framesPerSecondValue;
    
    [ObservableProperty]
    private int _quality;

    [ObservableProperty]
    private bool _hardwareAccelerationEnabled;
    
    [ObservableProperty]
    private bool _throttlingEnabled;

    [ObservableProperty]
    private bool _lowLatencyEnabled;
    
    public bool IsVBR => SelectedBitRateControl == BitrateControlMode.VBR;
    public Array VideoEncoderOptions => Enum.GetValues(typeof(VideoEncoderFormat));
    public Array BitRateControlOptions => Enum.GetValues(typeof(BitrateControlMode));
    public List<GameModeSettings> GameModeEnabled => ApplicationSettings.SettingsData.Value.EnabledGameModes;

    
    public SettingsViewModel()
    {
        var inputDevices = new List<AudioDevice> { new() { FriendlyName = "System Default" } };
        inputDevices.AddRange(Recorder.GetSystemAudioDevices(AudioDeviceSource.InputDevices));
        InputAudioDevices = inputDevices;
        
        var outputDevices = new List<AudioDevice> { new() { FriendlyName = "System Default" } };
        outputDevices.AddRange(Recorder.GetSystemAudioDevices(AudioDeviceSource.OutputDevices));
        OutputAudioDevices = outputDevices;
        
        var settings = ApplicationSettings.SettingsData.Value;
        _selectedInputDevice = InputAudioDevices.FirstOrDefault(d => d.FriendlyName == settings.SelectedInputDeviceName.DeviceName) ?? InputAudioDevices.First();
        _selectedOutputDevice = OutputAudioDevices.FirstOrDefault(d => d.FriendlyName == settings.SelectedOutputDeviceName.DeviceName) ?? OutputAudioDevices.First();
        _inputAudioVolume = settings.SelectedInputDeviceName.Volume;
        _outputAudioVolume = settings.SelectedOutputDeviceName.Volume;
        _bitrateValue = settings.Bitrate;
        _framesPerSecondValue = settings.FrameRate;
        _selectedVideoEncoderFormat = settings.Encoder;
        _selectedBitRateControl = settings.EncoderBitRateMode;
        _quality = settings.Quality;
        _hardwareAccelerationEnabled = settings.HardwareAcceleration;
        _throttlingEnabled = settings.ThrottlingEnabled;
        _lowLatencyEnabled = settings.LowLatencyEnabled;
        
        var anyGameModeChanged = this.GameModeEnabled
            .Select(item => item.WhenAnyValue(x => x.Enabled))
            .Merge();
        
        var settingsChanged = Observable.Merge(
            this.WhenAnyValue(x => x.SelectedInputDevice, x => x.SelectedOutputDevice, x => x.SelectedVideoEncoderFormat,
                x => x.ThrottlingEnabled, x => x.HardwareAccelerationEnabled, x => x.LowLatencyEnabled).Select(_ => Unit.Default),
            anyGameModeChanged.Select(_ => Unit.Default)
        );

        
        settingsChanged
            .Throttle(TimeSpan.FromMilliseconds(500))
            .Skip(1) 
            .ObserveOn(RxApp.MainThreadScheduler) 
            .Subscribe(_ => SaveSettings());
    }

    partial void OnSelectedInputDeviceChanged(AudioDevice value) => ApplicationSettings.SettingsData.Value.SelectedInputDeviceName.DeviceName = value?.FriendlyName;
    partial void OnSelectedOutputDeviceChanged(AudioDevice value) => ApplicationSettings.SettingsData.Value.SelectedOutputDeviceName.DeviceName = value?.FriendlyName;
    partial void OnInputAudioVolumeChanged(int value) => ApplicationSettings.SettingsData.Value.SelectedInputDeviceName.Volume = value;
    partial void OnOutputAudioVolumeChanged(int value) => ApplicationSettings.SettingsData.Value.SelectedOutputDeviceName.Volume = value;
    partial void OnBitrateValueChanged(int value) => ApplicationSettings.SettingsData.Value.Bitrate = value;
    partial void OnFramesPerSecondValueChanged(int value) => ApplicationSettings.SettingsData.Value.FrameRate = value;
    partial void OnSelectedVideoEncoderFormatChanged(VideoEncoderFormat value) => ApplicationSettings.SettingsData.Value.Encoder = value;
    partial void OnSelectedBitRateControlChanged(BitrateControlMode value) => ApplicationSettings.SettingsData.Value.EncoderBitRateMode = value;
    partial void OnQualityChanged(int value) => ApplicationSettings.SettingsData.Value.Quality = value;
    partial void OnHardwareAccelerationEnabledChanged(bool value) => ApplicationSettings.SettingsData.Value.HardwareAcceleration = value;
    partial void OnThrottlingEnabledChanged(bool value) => ApplicationSettings.SettingsData.Value.ThrottlingEnabled = value;
    partial void OnLowLatencyEnabledChanged(bool value) => ApplicationSettings.SettingsData.Value.LowLatencyEnabled = value;
    
    public void SaveSettings()
    {
        ApplicationSettings.SettingsData.Save("settings.json");
    }
}