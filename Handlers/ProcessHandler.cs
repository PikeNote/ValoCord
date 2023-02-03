using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ValCord.Handlers;

public static class ProcessHandler
{
    private static bool activeProcess = false;
    private static Timer pollTimer = new Timer();
    
    public static void Initialize() {  // Process timer for Valorant
        pollTimer.Interval = 500;
        pollTimer.Elapsed += ValorantProcessFound;
        pollTimer.AutoReset = true;
        pollTimer.Enabled = true;}

    static async void ValorantProcessFound(Object source, System.Timers.ElapsedEventArgs e)
    {
        if (ValorantAPI.CheckAuth())
        {
            Process[] pname = Process.GetProcessesByName("VALORANT");
            if (pname.Length == 0)
            {
                if (activeProcess)
                {
                    ValorantLogHandler.StopLogging();
                    ValorantAPI.ResetAuth();
                }
                activeProcess = false;
                
            }
            else
            {
                
                if (!activeProcess)
                {
                    activeProcess = true;
                    await ValorantAPI.reAuthAttempt();
                    ValorantLogHandler.StartLogging();
                    ValorantRecorder.SetWindowHandler();
                }
                
            }
        }
    }
}