namespace ValoCord.Handlers;

using System;
using System.Runtime.InteropServices;
using ScreenRecorderLib;

class WindowWatcher
{
    // WinEvent constants
    private const uint EVENT_SYSTEM_FOREGROUND = 0x0003;
    private const uint WINEVENT_OUTOFCONTEXT = 0x0000;

    // Delegate & hook handle
    private delegate void WinEventDelegate(
        IntPtr hWinEventHook, uint eventType,
        IntPtr hwnd, int idObject, int idChild,
        uint dwEventThread, uint dwmsEventTime);

    private readonly WinEventDelegate _winEventProc;
    private IntPtr _hook;
    private IntPtr _targetHwnd;

    public WindowWatcher(IntPtr targetHwnd)
    {
        _targetHwnd   = targetHwnd;
        _winEventProc = WinEventProc;
    }

    public void Start()
    {
        _hook = SetWinEventHook(
            EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND,
            IntPtr.Zero,
            _winEventProc,
            0, 0,
            WINEVENT_OUTOFCONTEXT);
    }

    public void Stop()
    {
        if (_hook != IntPtr.Zero)
            UnhookWinEvent(_hook);
    }

    private void WinEventProc(
        IntPtr hWinEventHook, uint eventType,
        IntPtr hwnd, int idObject, int idChild,
        uint dwEventThread, uint dwmsEventTime)
    {
        if (hwnd == _targetHwnd)
        {
            ValorantRecorder.EnableSource();
        }
        else
        {
            ValorantRecorder.DisableSource();
        }
    }
    

    #region P/Invoke
    [DllImport("user32.dll")]
    private static extern IntPtr SetWinEventHook(
        uint eventMin, uint eventMax,
        IntPtr hmodWinEventProc,
        WinEventDelegate lpfnWinEventProc,
        uint idProcess, uint idThread,
        uint dwFlags);

    [DllImport("user32.dll")]
    private static extern bool UnhookWinEvent(IntPtr hWinEventHook);
    #endregion
}