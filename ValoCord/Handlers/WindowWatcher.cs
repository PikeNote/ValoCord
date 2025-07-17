using NLog;
// ReSharper disable InconsistentNaming

namespace ValoCord.Handlers;

using System;
using System.Runtime.InteropServices;

partial class WindowWatcher
{
    private readonly Logger _logger = LogManager.GetLogger("Window Watcher");
    
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
    private readonly IntPtr _targetHwnd;
    
    private Thread? _messageLoopThread;
    private bool _running;


    public WindowWatcher(IntPtr targetHwnd)
    {
        _targetHwnd   = targetHwnd;
        _winEventProc = WinEventProc;
    }

    public void Start()
    {
        if (_running)
            return;
        _running = true;
        _messageLoopThread = new Thread(() =>
        {
            _hook = SetWinEventHook(
                EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND,
                IntPtr.Zero,
                _winEventProc,
                0, 0,
                WINEVENT_OUTOFCONTEXT);
            
            if (_hook == IntPtr.Zero)
            {
                int error = Marshal.GetLastWin32Error();
                _logger.Error($"Failed to set WinEventHook. Error: {error}");
                _running = false;
                return;
            }

            _logger.Info("Window Watcher started");
            
            MSG msg;
            while (_running && GetMessage(out msg, IntPtr.Zero, 0, 0))
            {
                TranslateMessage(ref msg);
                DispatchMessage(ref msg);
            }
            
            if (_hook != IntPtr.Zero)
            {
                UnhookWinEvent(_hook);
                _hook = IntPtr.Zero;
            }

            _logger.Info("Window Watcher stopped");
        });
        _messageLoopThread.SetApartmentState(ApartmentState.STA);
        _messageLoopThread.IsBackground = true;
        _messageLoopThread.Start();
    }

    public void Stop()
    {
        if (!_running)
            return;

        _running = false;

        if (_messageLoopThread != null)
        {
            PostThreadMessage(GetThreadId(_messageLoopThread), WM_QUIT, IntPtr.Zero, IntPtr.Zero);

            _messageLoopThread.Join();
        }

        _messageLoopThread = null;
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
    private const int WM_QUIT = 0x0012;
    
    [StructLayout(LayoutKind.Sequential)]
    private struct MSG
    {
        public IntPtr hwnd;
        public uint message;
        public UIntPtr wParam;
        public IntPtr lParam;
        public uint time;
        public Point pt;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Point
    {
        public int x;
        public int y;
    }
    
    [DllImport("user32.dll")]
    private static extern bool GetMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax); 

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool TranslateMessage(ref MSG lpMsg);

    [LibraryImport("user32.dll", EntryPoint = "DispatchMessageW")]
    private static partial IntPtr DispatchMessage(ref MSG lpMsg);

    [DllImport("user32.dll")]
    private static extern IntPtr SetWinEventHook(
        uint eventMin, uint eventMax,
        IntPtr hmodWinEventProc,
        WinEventDelegate lpfnWinEventProc,
        uint idProcess, uint idThread,
        uint dwFlags);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool UnhookWinEvent(IntPtr hWinEventHook);

    [LibraryImport("user32.dll", EntryPoint = "PostThreadMessageW")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool PostThreadMessage(uint idThread, uint MSG, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll")]
    private static extern uint GetThreadId(Thread thread);
    #endregion
}