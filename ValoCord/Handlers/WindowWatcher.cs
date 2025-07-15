using NLog;

namespace ValoCord.Handlers;

using System;
using System.Runtime.InteropServices;

class WindowWatcher
{
    Logger logger = LogManager.GetLogger("Window Watcher");
    
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
    
    private Thread _messageLoopThread;
    private bool _running;


    public WindowWatcher(IntPtr targetHwnd)
    {
        _targetHwnd   = targetHwnd;
        _winEventProc = new WinEventDelegate(WinEventProc);
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
                logger.Error($"Failed to set WinEventHook. Error: {error}");
                _running = false;
                return;
            }

            logger.Info("Window Watcher started");
            
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

            logger.Info("Window Watcher stopped");
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

        PostThreadMessage(GetThreadId(_messageLoopThread), WM_QUIT, IntPtr.Zero, IntPtr.Zero);
        
        _messageLoopThread.Join();
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
        public POINT pt;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT
    {
        public int x;
        public int y;
    }
    
    [DllImport("user32.dll")]
    private static extern bool GetMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

    [DllImport("user32.dll")]
    private static extern bool TranslateMessage(ref MSG lpMsg);

    [DllImport("user32.dll")]
    private static extern IntPtr DispatchMessage(ref MSG lpMsg);

    [DllImport("user32.dll")]
    private static extern IntPtr SetWinEventHook(
        uint eventMin, uint eventMax,
        IntPtr hmodWinEventProc,
        WinEventDelegate lpfnWinEventProc,
        uint idProcess, uint idThread,
        uint dwFlags);

    [DllImport("user32.dll")]
    private static extern bool UnhookWinEvent(IntPtr hWinEventHook);

    [DllImport("user32.dll")]
    private static extern bool PostThreadMessage(uint idThread, uint Msg, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll")]
    private static extern uint GetThreadId(Thread thread);
    #endregion
}