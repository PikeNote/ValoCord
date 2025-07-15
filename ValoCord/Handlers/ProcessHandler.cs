using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ValoCord.Handlers;

public static class ProcessHandler
{
    private static bool _activeProcess = false;
    private static System.Timers.Timer _pollTimer = new();
    
    public static void Initialize() {  // Process timer for Valorant
        _pollTimer.Interval = 500;
        _pollTimer.Elapsed += ValorantProcessFound;
        _pollTimer.AutoReset = true;
        _pollTimer.Enabled = true;
        
    }

    static async void ValorantProcessFound(Object source, System.Timers.ElapsedEventArgs e)
    {
        if (ValorantAPI.CheckAuth())
        {
            Process[] pname = Process.GetProcessesByName("VALORANT");
            if (pname.Length == 0)
            {
                if (_activeProcess)
                {
                    ProgramStatusHandler.Instance.CurrentStatus = ProgramStatusHandler.ValorantNotOpen;
                    Console.WriteLine("VALORANT closed!");
                    ValorantLogHandler.StopLogging();
                    ValorantAPI.ResetAuth();
                }
                _activeProcess = false;
                
            }
            else
            {
                Console.WriteLine("VALORANT found!");
                if (_activeProcess) return;
                ProgramStatusHandler.Instance.CurrentStatus = ProgramStatusHandler.WaitingForGame;
                _activeProcess = true;
                await ValorantAPI.reAuthAttempt();
                ValorantLogHandler.StartLogging();

            }
        }
    }
}