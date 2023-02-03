using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using NLog;
using ScreenRecorderLib;

namespace ValCord.Handlers;

public static class ValorantRecorder
{
    private static bool _recordingInProgress = false;
    // ReSharper disable once InconsistentNaming
    private static IntPtr ValorantWindowHandler = IntPtr.Zero;
    private static readonly string path = Path.Combine(Environment.GetFolderPath(
        Environment.SpecialFolder.LocalApplicationData), "ValoCord\\recorder.log");
    private static readonly string DefaultVideoPath = Path.Combine(Environment.GetFolderPath(
        Environment.SpecialFolder.LocalApplicationData), "ValoCord\\");
    static Logger logger = LogManager.GetLogger("Video Recordinng");
    

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
        List<RecordingSourceBase> rdSources = new List<RecordingSourceBase>();
        if (ValorantWindowHandler != IntPtr.Zero)
        {
            rdSources.Add(new WindowRecordingSource(ValorantWindowHandler));
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
                    LogFilePath = path,
                    LogSeverityLevel = ScreenRecorderLib.LogLevel.Debug
                }
            };

            Recorder rec = Recorder.CreateRecorder(options);
            rd = rec;
            
            rec.OnRecordingComplete += Rec_OnRecordingComplete;
            rec.OnRecordingFailed += Rec_OnRecordingFailed;
            rec.OnStatusChanged += Rec_OnStatusChanged;
            
            String videoPath = Path.Combine(DefaultVideoPath, $"{fileName}.mp4");
            rec.Record(videoPath);
            
            

        }
            
       
    }
    public static void StopRecording()
    {
        rd.Stop(); 
    }
    private static void Rec_OnRecordingComplete(object? sender, RecordingCompleteEventArgs e)
    {
        //Get the file path if recorded to a file
        string path = e.FilePath;	
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