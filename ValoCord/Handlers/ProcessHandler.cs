using System.Diagnostics;

namespace ValoCord.Handlers;

public static class ProcessHandler
{
    private static bool _activeProcess;
    private static readonly System.Timers.Timer PollTimer = new();
    
    public static void Initialize() {  // Process timer for VALORANT
        PollTimer.Interval = 500;
        PollTimer.Elapsed += ValorantProcessFound;
        PollTimer.AutoReset = true;
        PollTimer.Enabled = true;
        
    }

    private static async void ValorantProcessFound(object? source, System.Timers.ElapsedEventArgs e)
    {
        try
        {
            if (!ValorantApi.CheckAuth()) return;
            var pname = Process.GetProcessesByName("VALORANT");
            if (pname.Length == 0)
            {
                if (_activeProcess)
                {
                    ProgramStatusHandler.Instance.CurrentStatus = ProgramStatusHandler.ValorantNotOpen;
                    Console.WriteLine("VALORANT closed!");
                    await ValorantLogHandler.StopLogging();
                    ValorantApi.ResetAuth();
                }
                _activeProcess = false;
                
            }
            else
            {
                Console.WriteLine("VALORANT found!");
                if (_activeProcess) return;
                ProgramStatusHandler.Instance.CurrentStatus = ProgramStatusHandler.WaitingForGame;
                _activeProcess = true;
                await ValorantApi.ReAuthAttempt();
                ValorantLogHandler.StartLogging();

            }
        }
        catch (Exception ex)
        {
            Console.Write($"Error while checking for valorant: {ex.Message}");
        }
    }
}