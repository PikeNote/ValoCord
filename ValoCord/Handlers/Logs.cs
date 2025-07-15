using System;
using System.IO;
using NLog;

namespace ValoCord.Handlers;

public static class Logs
{
    public static void Initialize() { 
        DirectoryInfo logDir = System.IO.Directory.CreateDirectory(Paths.ValoCordPath);
        System.IO.Directory.CreateDirectory(Path.Combine(Paths.ValoCordPath,"data"));
        
        var config = new NLog.Config.LoggingConfiguration();

// Targets where to log to: File and Console
        var logfile = new NLog.Targets.FileTarget("logfile") { FileName = Path.Combine(Paths.ValoCordPath,"log.txt"), MaxArchiveFiles = 10};
        var logconsole = new NLog.Targets.ConsoleTarget("logconsole");
            
        //
        // c:\\temp\\log-internal.txt
        
// Rules for mapping loggers to targets            
        config.AddRule(LogLevel.Debug, LogLevel.Fatal, logconsole);
        config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);
        
        
        ;
            
// Apply config           
        NLog.LogManager.Configuration = config;
        
        System.Diagnostics.Debug.WriteLine("Logging started");
        
        Logger logger = LogManager.GetLogger("ValoCord");
        logger.Info("Program started");
        
    }
}