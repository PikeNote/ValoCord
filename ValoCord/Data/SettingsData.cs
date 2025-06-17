using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ReactiveUI;
using ScreenRecorderLib;

namespace ValoCord.Data;

public class SettingsData
{
    public AudioDevice SelectedInputDeviceName { get; set; } = new AudioDevice()
    {
        DeviceName = "System Default",
        Volume = 50
    };
    public AudioDevice SelectedOutputDeviceName { get; set; } = new AudioDevice()
    {
        DeviceName = "System Default",
        Volume = 50
    };

    [JsonConverter(typeof(StringEnumConverter))]
    public VideoEncoderFormat  Encoder {get; set;} = VideoEncoderFormat.H264;
    
    [JsonConverter(typeof(StringEnumConverter))]
    public BitrateControlMode  EncoderBitRateMode {get; set;} = BitrateControlMode.CBR;
    
    public Boolean HardwareAcceleration { get; set; } = true;

    public Boolean ThrottlingEnabled { get; set; } = true;

    public Boolean IsLowLatencyEnabled { get; set; } = false;
    
    public int Bitrate { get; set; } = 96000;
    
    public int FrameRate { get; set; } = 60;

    public int Quality { get; set; } = 70;

    public List<GameModeSettings> EnabledGameModes { get; set; } = GameModes.GetDefaultGameModeSettings();
    
    
    
    public VideoEncoderOptions CreateVideoEncoder()
    {
        VideoEncoderOptions vdOptions = new VideoEncoderOptions
        {
            Bitrate = Bitrate,
            Framerate = FrameRate,
            IsFixedFramerate = true,
            IsThrottlingDisabled = ThrottlingEnabled,
            IsHardwareEncodingEnabled = HardwareAcceleration,
            IsLowLatencyEnabled = IsLowLatencyEnabled,
            Quality = Quality,
        };
        switch (Encoder)
        {
            case VideoEncoderFormat.H264:
                vdOptions.Encoder = new H264VideoEncoder
                {
                    BitrateMode = (EncoderBitRateMode == BitrateControlMode.VBR) ? H264BitrateControlMode.Quality : H264BitrateControlMode.CBR,
                    EncoderProfile = H264Profile.Main,
                };
                break;

            case VideoEncoderFormat.H265:
                vdOptions.Encoder = new H265VideoEncoder
                {
                    BitrateMode = (EncoderBitRateMode == BitrateControlMode.VBR) ? H265BitrateControlMode.Quality : H265BitrateControlMode.CBR,
                    EncoderProfile = H265Profile.Main,
                };
                break;

            default:
                vdOptions.Encoder = new H264VideoEncoder
                {
                    BitrateMode = (EncoderBitRateMode == BitrateControlMode.VBR) ? H264BitrateControlMode.Quality : H264BitrateControlMode.CBR,
                    EncoderProfile = H264Profile.Main,
                };
                break;
        }
        return vdOptions;
    }

}

public class AudioDevice
{
    public string DeviceName { get; set; }
    public int Volume { get; set; }
    
}

public enum VideoCodec
{
    H264,
    H265
}

public enum BitrateControlMode
{
    CBR,
    VBR
}

public class GameModeSettings
{
    public string GameMode { get; set; }
    public Boolean Enabled { get; set; }
}