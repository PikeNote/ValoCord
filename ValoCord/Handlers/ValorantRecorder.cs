using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using NLog;
using ScreenRecorderLib;

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
    private static WindowWatcher winWatcher = new WindowWatcher(ValorantWindowHandler);

    private static Recorder rd;
    public static void SetWindowHandler()
    {
        foreach (Process pList in Process.GetProcesses())
        {
            if (pList.MainWindowTitle.Contains("VALORANT"))
            {
                ValorantWindowHandler = pList.MainWindowHandle;
            }
        }
        
    }
    
    public static void StartRecording(String fileName)
    {
        logger.Info("Video recording request: " + fileName);

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
                },
                VideoEncoderOptions = new VideoEncoderOptions
                {
                    Quality = 50,
                    Framerate = 144,
                    IsFixedFramerate = false,
                    Encoder = new H264VideoEncoder()
                    {
                        BitrateMode = H264BitrateControlMode.Quality,
                        EncoderProfile = H264Profile.Main,

                    },
                    IsFragmentedMp4Enabled = true,
                    IsThrottlingDisabled = false,
                    IsHardwareEncodingEnabled = true,
                    IsLowLatencyEnabled = false,
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
            rec.Record(videoPath);
            winWatcher.Start();
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
        winWatcher.Stop();
    }
    private static void Rec_OnRecordingFailed(object? sender, RecordingFailedEventArgs e)
    {
        logger.Info(e.Error);
    }
    private static void Rec_OnStatusChanged(object? sender, RecordingStatusEventArgs e)
    {
        if (e.Status == RecorderStatus.Recording)
        {
            _recordingInProgress = true;
            logger.Info("Recording started");
        }
    }
}