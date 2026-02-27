using System;
using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming

namespace TrayIconRunner.Util;

public static class WinEventHookUtils {

    public delegate void WinEventDelegate(
        IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, 
        uint dwEventThread, uint dwmsEventTime
    );

    [DllImport("user32.dll")]
    public static extern IntPtr SetWinEventHook(
        uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc,
        uint idProcess, uint idThread, uint dwFlags
    );

    /// <summary>
    /// 显示/隐藏窗口
    /// <p>
    /// 第二个参数表示对窗口进行的操作，详见https://www.bbsmax.com/A/A2dmnAVOze/
    /// </p>
    /// </summary>
    [DllImport("user32.dll")]
    public static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);

    [DllImport("user32.dll")]
    public static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern bool UnhookWinEvent(IntPtr hWinEventHook);
}
