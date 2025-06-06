using System;
using Avalonia.Controls;
using LibVLCSharp.Avalonia;
using LibVLCSharp.Shared;

namespace ValoCord.Video;

public class VLCPlayerService
{
    private LibVLC MainLibVLC { get; set; }
    private MediaPlayer MainMediaPlayer { get; set; }

    public Control CreateControl()
    {
        // Create player
        MainLibVLC = new LibVLC(enableDebugLogs: false);

        // Create player view
        MainMediaPlayer = new(MainLibVLC);

        // Create player control
        VideoView videoView = new()
        {
            MediaPlayer = MainMediaPlayer
        };

        return videoView;
    }

    public void Play(string uri)
    {
        // Create media
        var media = new Media(MainLibVLC, new Uri(uri));

        // Play media
        MainMediaPlayer.Media = media;
        MainMediaPlayer.Play();
    }
}