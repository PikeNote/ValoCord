using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Threading.Tasks;
using LibVLCSharp.Shared;
using ReactiveUI;

namespace ValoCord.ViewModels;

public class VODViewerViewModel : ViewModelBase {
    private double _progress = 0.0;
    
    LibVLC? _libVLC;
    public MediaPlayer? MediaPlayer;
        
    public double Progress
    { 
        get => _progress; 
        set => this.RaiseAndSetIfChanged(ref _progress, value); 
    }
    
    public ObservableCollection<Person> Items { get; }

    public Boolean IsSeeking = false;

    public VODViewerViewModel()
    {
        
        if (!Avalonia.Controls.Design.IsDesignMode)
        {
            _libVLC = new LibVLC(
                enableDebugLogs: false
            );
            
            MediaPlayer = new MediaPlayer(_libVLC) { };
            
            
            Observable.FromEventPattern<MediaPlayerPositionChangedEventArgs>(
                    handler => MediaPlayer.PositionChanged += handler,
                    handler => MediaPlayer.PositionChanged -= handler)
                .Sample(TimeSpan.FromSeconds(0.1))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x => { Progress = Math.Clamp( x.EventArgs.Position, 0, 100.0); });
        }
        
        Items = new ObservableCollection<Person>
        {
            new Person { Name = "1" },
            new Person { Name = "2" },
            new Person { Name = "3" },
            new Person { Name = "4" },
            new Person { Name = "5" },
            new Person { Name = "6" },
            new Person { Name = "7" },
            new Person { Name = "8" },
            new Person { Name = "9" },
            new Person { Name = "10" },
            // etc.
        };
    }

    public async Task DoPlay()
    {
        using var media = new Media(
            _libVLC,
            new Uri("https://file-examples.com/storage/fe32c8d6966839f839df247/2017/04/file_example_MP4_480_1_5MG.mp4") //new Uri("http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4"),
        );

        MediaPlayer.Play(media);
        media.Dispose();
    }
    
    public class Person
    {
        public string Name { get; set; }
        // other properties...
    }
}