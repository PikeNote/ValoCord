using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using NLog;
using ScreenRecorderLib;
using ValoCord.Data;
namespace ValoCord.Handlers;

public static class ValorantRecorder
{
    [DllImport("user32.dll")] 
    static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFOEX lpmi);

    [StructLayout(LayoutKind.Sequential)]
    struct RECT { public int Left, Top, Right, Bottom; }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    struct MONITORINFOEX {
        public int cbSize;
        public RECT rcMonitor;
        public RECT rcWork;
        public uint dwFlags;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string szDevice;
    }
    
    private static bool _recordingInProgress = false;
    // ReSharper disable once InconsistentNaming
    private static IntPtr ValorantWindowHandler = IntPtr.Zero;
    private static Logger logger = LogManager.GetLogger("Video Recordinng");
    private static DisplayRecordingSource dispRecordingSource = null;
    private static WindowWatcher? _winWatcher;

    private static Recorder rd;
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
        logger.Info("Video recording request: " + fileName);
        await SetWindowHandler();

        GetWindowRect(ValorantWindowHandler, out RECT winRect);
        int winWidth  = winRect.Right  - winRect.Left;
        int winHeight = winRect.Bottom - winRect.Top;
        
        var hMon = MonitorFromWindow(ValorantWindowHandler, 2);
        var mi   = new MONITORINFOEX { cbSize = Marshal.SizeOf<MONITORINFOEX>() };
        GetMonitorInfo(hMon, ref mi);
        
        int offsetX = winRect.Left - mi.rcMonitor.Left;
        int offsetY = winRect.Top  - mi.rcMonitor.Top;
        
        if (ValorantWindowHandler != IntPtr.Zero)
        { 
            List<RecordingSourceBase> rdSources = new List<RecordingSourceBase>();
            dispRecordingSource = new DisplayRecordingSource
            {
                DeviceName = DisplayRecordingSource.MainMonitor.DeviceName,
                RecorderApi = RecorderApi.DesktopDuplication,
                SourceRect = new ScreenRect(offsetX, offsetY, winWidth, winHeight),
                IsVideoCaptureEnabled = true
            };
            
            rdSources.Add(dispRecordingSource);
            
            List<ScreenRecorderLib.AudioDevice> inputDevices = Recorder.GetSystemAudioDevices(AudioDeviceSource.InputDevices);
            List<ScreenRecorderLib.AudioDevice> outputDevices = Recorder.GetSystemAudioDevices(AudioDeviceSource.OutputDevices);

            String inputAudioDevice = "";
            String outputAudioDevice = "";

            if (ApplicationSettings.SettingsData.Value.SelectedInputDeviceName.DeviceName != "System Default")
            {
                var audioDevice = inputDevices.Find(dev => dev.FriendlyName == ApplicationSettings.SettingsData.Value.SelectedInputDeviceName.DeviceName);
                if (audioDevice == null)
                {
                    ApplicationSettings.SettingsData.Value.ResetInputDevice();
                    ApplicationSettings.SettingsData.Save("settings.json");
                }
                else
                {
                    inputAudioDevice = audioDevice.DeviceName;
                }
            }
            
            if (ApplicationSettings.SettingsData.Value.SelectedOutputDeviceName.DeviceName != "System Default")
            {
                var audioDevice = inputDevices.Find(dev => dev.FriendlyName == ApplicationSettings.SettingsData.Value.SelectedOutputDeviceName.DeviceName);
                if (audioDevice == null)
                {
                    ApplicationSettings.SettingsData.Value.ResetInputDevice();
                    ApplicationSettings.SettingsData.Save("settings.json");
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
                    InputVolume = ApplicationSettings.SettingsData.Value.SelectedInputDeviceName.Volume,
                    OutputVolume = ApplicationSettings.SettingsData.Value.SelectedOutputDeviceName.Volume,
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
                    IsLowLatencyEnabled = ApplicationSettings.SettingsData.Value.IsLowLatencyEnabled,
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

            Recorder rec = Recorder.CreateRecorder(options);
            rd = rec;
            
            rec.OnRecordingComplete += Rec_OnRecordingComplete;
            rec.OnRecordingFailed += Rec_OnRecordingFailed;
            rec.OnStatusChanged += Rec_OnStatusChanged;
            
            String videoPath = Path.Combine(Paths.DefaultVideoPath, $"{fileName}.mp4");
            _winWatcher.Start();
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
        
        dispRecordingSource.IsVideoCaptureEnabled = false;
        rd.GetDynamicOptionsBuilder()
            .SetUpdatedRecordingSource(dispRecordingSource)
            .Apply();
    }
    
    public static void EnableSource()
    {
        if(!_recordingInProgress) { return; }
        
        dispRecordingSource.IsVideoCaptureEnabled = true;
        rd.GetDynamicOptionsBuilder()
            .SetUpdatedRecordingSource(dispRecordingSource)
            .Apply();
    }

    
    public static void StopRecording()
    {
        rd.Stop(); 
    }
    private static void Rec_OnRecordingComplete(object? sender, RecordingCompleteEventArgs e)
    {
        //Get the file path if recorded to a file
        string path = e.FilePath;
        if (_winWatcher != null)
        {
            _winWatcher.Stop();
        }
    }
    private static void Rec_OnRecordingFailed(object? sender, RecordingFailedEventArgs e)
    {
        logger.Info(e.Error);
    }
    private static void Rec_OnStatusChanged(object? sender, RecordingStatusEventArgs e)
    {
        if (e.Status == RecorderStatus.Recording)
        {
            ProgramStatusHandler.CurrentStatus = ProgramStatusHandler.RecordingInProgress;
            _recordingInProgress = true;
            logger.Info("Recording started");
        }
        else
        {
            if (ProgramStatusHandler.CurrentStatus == ProgramStatusHandler.RecordingInProgress)
            {
                ProgramStatusHandler.CurrentStatus = ProgramStatusHandler.WaitingForGame;
            }
        }
    }
}