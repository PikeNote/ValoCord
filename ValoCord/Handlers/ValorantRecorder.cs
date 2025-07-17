using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using NLog;
using ScreenRecorderLib;

namespace ValoCord.Handlers;

public static partial class ValorantRecorder
{
    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool GetWindowRect(IntPtr hWnd, out Rect lpRect);
    [LibraryImport("user32.dll")]
    private static partial IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern bool GetMonitorInfo(IntPtr hMonitor, ref Monitorinfoex lpmi);

    [StructLayout(LayoutKind.Sequential)]
    private struct Rect { public int Left, Top, Right, Bottom; }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private struct Monitorinfoex {
        public int cbSize;
        public Rect rcMonitor;
        public Rect rcWork;
        public uint dwFlags;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string szDevice;
    }
    
    private static bool _recordingInProgress = false;
    // ReSharper disable once InconsistentNaming
    private static IntPtr ValorantWindowHandler = IntPtr.Zero;
    private static readonly Logger Logger = LogManager.GetLogger("Video Recording");
    private static DisplayRecordingSource? _dispRecordingSource;
    private static WindowWatcher? _winWatcher;

    private static Recorder? _rd;
    public static async Task SetWindowHandler(int maxRetries = 5, int delayMs = 500)
    {
        int attempt = 0;

        while (attempt < maxRetries)
        {
            foreach (Process pList in Process.GetProcesses())
            {
                if (pList.MainWindowTitle.Contains("VALORANT"))
                {
                    ValorantWindowHandler = pList.MainWindowHandle;
                    _winWatcher = new WindowWatcher(ValorantWindowHandler);
                        
                    if (ValorantWindowHandler != IntPtr.Zero)
                    {
                        return;
                    }
                    Console.WriteLine("Window handle not grabbed! Trying again...");
                }
            } 
            attempt++;
            await Task.Delay(delayMs);
        }
    }
    
    public static async Task StartRecording(String fileName)
    {
        Logger.Info("Video recording request: " + fileName);
        await SetWindowHandler();

        GetWindowRect(ValorantWindowHandler, out Rect winRect);
        int winWidth  = winRect.Right  - winRect.Left;
        int winHeight = winRect.Bottom - winRect.Top;
        
        var hMon = MonitorFromWindow(ValorantWindowHandler, 2);
        var mi   = new Monitorinfoex { cbSize = Marshal.SizeOf<Monitorinfoex>() };
        GetMonitorInfo(hMon, ref mi);
        
        int offsetX = winRect.Left - mi.rcMonitor.Left;
        int offsetY = winRect.Top  - mi.rcMonitor.Top;
        
        if (ValorantWindowHandler != IntPtr.Zero)
        { 
            List<RecordingSourceBase?> rdSources = [];
            _dispRecordingSource = new DisplayRecordingSource
            {
                DeviceName = DisplayRecordingSource.MainMonitor.DeviceName,
                RecorderApi = ApplicationSettings.SettingsData.Value.RecorderApi,
                SourceRect = new ScreenRect(offsetX, offsetY, winWidth, winHeight),
                IsVideoCaptureEnabled = true,
                IsBorderRequired = ApplicationSettings.SettingsData.Value.IsBorderEnabled
            };
            
            rdSources.Add(_dispRecordingSource);
            
            List<AudioDevice> inputDevices = Recorder.GetSystemAudioDevices(AudioDeviceSource.InputDevices);
            List<AudioDevice> outputDevices = Recorder.GetSystemAudioDevices(AudioDeviceSource.OutputDevices);

            String inputAudioDevice = "";
            String outputAudioDevice = "";

            if (ApplicationSettings.SettingsData.Value.SelectedInputDeviceName.DeviceName != "System Default")
            {
                var audioDevice = inputDevices.Find(dev => dev.FriendlyName == ApplicationSettings.SettingsData.Value.SelectedInputDeviceName.DeviceName);
                if (audioDevice == null)
                {
                    ApplicationSettings.SettingsData.Value.ResetInputDevice();
                    await ApplicationSettings.SettingsData.Save("settings.json");
                }
                else
                {
                    inputAudioDevice = audioDevice.DeviceName;
                }
            }
            
            if (ApplicationSettings.SettingsData.Value.SelectedOutputDeviceName.DeviceName != "System Default")
            {
                var audioDevice = outputDevices.Find(dev => dev.FriendlyName == ApplicationSettings.SettingsData.Value.SelectedOutputDeviceName.DeviceName);
                if (audioDevice == null)
                {
                    ApplicationSettings.SettingsData.Value.ResetInputDevice();
                    await ApplicationSettings.SettingsData.Save("settings.json");
                }
                else
                {
                    outputAudioDevice = audioDevice.DeviceName;
                }
            }

            RecorderOptions options = new RecorderOptions
            {
                SourceOptions = new SourceOptions
                {
                    RecordingSources = rdSources
                },
                OutputOptions = new OutputOptions
                {
                    RecorderMode = RecorderMode.Video,
                    OutputFrameSize = new ScreenSize(1920, 1080),
                    Stretch = StretchMode.Uniform
                },
                AudioOptions = new AudioOptions
                {
                    Bitrate = AudioBitrate.bitrate_128kbps,
                    Channels = AudioChannels.Stereo,
                    IsAudioEnabled = true,
                    AudioInputDevice = inputAudioDevice,
                    AudioOutputDevice = outputAudioDevice,
                    InputVolume = ApplicationSettings.SettingsData.Value.SelectedInputDeviceName.Volume / 100,
                    OutputVolume = ApplicationSettings.SettingsData.Value.SelectedOutputDeviceName.Volume / 100,
                },
                VideoEncoderOptions = new VideoEncoderOptions
                {
                    Bitrate = ApplicationSettings.SettingsData.Value.Bitrate,
                    Framerate = ApplicationSettings.SettingsData.Value.FrameRate,
                    IsFixedFramerate = false,
                    Encoder = ApplicationSettings.SettingsData.Value.CreateVideoEncoder(),
                    IsFragmentedMp4Enabled = true,
                    IsThrottlingDisabled = !ApplicationSettings.SettingsData.Value.ThrottlingEnabled,
                    IsHardwareEncodingEnabled = ApplicationSettings.SettingsData.Value.HardwareAcceleration,
                    IsLowLatencyEnabled = ApplicationSettings.SettingsData.Value.LowLatencyEnabled,
                    IsMp4FastStartEnabled = false
                },
                MouseOptions = new MouseOptions
                {
                    IsMouseClicksDetected = false
                },
                LogOptions = new LogOptions
                {
                    IsLogEnabled = true,
                    LogFilePath = Paths.RecordingLogPath,
                    LogSeverityLevel = ScreenRecorderLib.LogLevel.Debug
                }
            };

            Recorder? rec = Recorder.CreateRecorder(options);
            _rd = rec;
            
            rec.OnRecordingComplete += Rec_OnRecordingComplete;
            rec.OnRecordingFailed += Rec_OnRecordingFailed;
            rec.OnStatusChanged += Rec_OnStatusChanged;
            
            String videoPath = Path.Combine(Paths.DefaultVideoPath, $"{fileName}.mp4");
            _winWatcher?.Start();
            rec.Record(videoPath);
        }
        else
        {
            Console.WriteLine("Video recording request failed; Window not grabbed?");
        }
            
       
    }

    public static void DisableSource()
    {
        if(!_recordingInProgress) { return; }

        if (_dispRecordingSource == null) return;
        _dispRecordingSource.IsVideoCaptureEnabled = false;
        _rd?.GetDynamicOptionsBuilder()
            .SetUpdatedRecordingSource(_dispRecordingSource)
            .Apply();
    }
    
    public static void EnableSource()
    {
        if(!_recordingInProgress) { return; }

        if (_dispRecordingSource == null) return;
        _dispRecordingSource.IsVideoCaptureEnabled = true;
        _rd?.GetDynamicOptionsBuilder()
            .SetUpdatedRecordingSource(_dispRecordingSource)
            .Apply();
    }

    
    public static void StopRecording()
    {
        _rd?.Stop(); 
    }
    private static void Rec_OnRecordingComplete(object? sender, RecordingCompleteEventArgs e)
    {
        //Get the file path if recorded to a file
        var path = e.FilePath;
        _winWatcher?.Stop();
    }
    private static void Rec_OnRecordingFailed(object? sender, RecordingFailedEventArgs e)
    {
        Logger.Info(e.Error);
    }
    private static void Rec_OnStatusChanged(object? sender, RecordingStatusEventArgs e)
    {
        if (e.Status == RecorderStatus.Recording)
        {
            ProgramStatusHandler.Instance.CurrentStatus = ProgramStatusHandler.RecordingInProgress;
            _recordingInProgress = true;
            Logger.Info("Recording started");
        }
        else
        {
            if (ProgramStatusHandler.Instance.CurrentStatus == ProgramStatusHandler.RecordingInProgress)
            {
                ProgramStatusHandler.Instance.CurrentStatus = ProgramStatusHandler.WaitingForGame;
            }
        }
    }
}