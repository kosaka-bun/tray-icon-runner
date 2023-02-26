using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using TrayIconRunner.Util;

// ReSharper disable MemberCanBeMadeStatic.Global
// ReSharper disable MemberCanBeMadeStatic.Local
// ReSharper disable InconsistentNaming

namespace TrayIconRunner {

public class Launcher {
    
    /// <summary>
    /// 专有文件的后缀名
    /// </summary>
    private const string suffix = ".tir";
    
    private readonly string filePath;

    public Process process;

    public bool launched;

    private IntPtr minimizeEventHookId;

    private Thread winEventListener;

    public Launcher(string filePath) {
        this.filePath = filePath;
    }

    public void launch() {
        //读取专有文件
        string content = Utils.readFileToString(filePath);
        //如果文件为空，则调用该文件所在目录下，与该文件同名的，后缀名为.exe的文件
        #region
        string exePath, iconName = null, fileToOpen;
        if(content.Length <= 0) {
            fileToOpen = exePath = filePath.Substring(0, 
                filePath.Length - suffix.Length) + ".exe";
            if(!File.Exists(exePath)) {
                Utils.messageBox($"{exePath} 文件不存在", MessageBoxIcon.Error);
                Application.Exit();
                return;
            }
            initProcess(exePath);
        } else {
            //解析专有文件
            JObject jo = JObject.Parse(content);
            iconName = jo["name"]?.Value<string>().Trim();
            fileToOpen = jo["file"]?.Value<string>().Trim();
            if(fileToOpen == null) {
                Utils.messageBox("没有提供要打开的文件", MessageBoxIcon.Error);
                Application.Exit();
                return;
            }
            if(!fileToOpen.Contains(":\\")) {
                fileToOpen = Utils.calcAbsolutePath(filePath,
                    fileToOpen);
            }
            if(!File.Exists(fileToOpen)) {
                Utils.messageBox($"{fileToOpen} 文件不存在", MessageBoxIcon.Error);
                Application.Exit();
                return;
            }
            //读取指定扩展名的关联程序路径
            int pointIndex = fileToOpen.LastIndexOf(".", 
                StringComparison.Ordinal);
            if(pointIndex == -1) {
                Utils.messageBox($"未找到 {fileToOpen} 的关联程序", 
                    MessageBoxIcon.Error);
                Application.Exit();
                return;
            }
            string extName = fileToOpen.Substring(pointIndex);
            exePath = Utils.getAssociatedProgramPath(extName);
            // ReSharper disable once ConvertIfStatementToNullCoalescingExpression
            if(exePath == null) {
                exePath = AssociatedPrograms.get(extName);
            }
            if(exePath == null) {
                Utils.messageBox($"未找到 {fileToOpen} 的关联程序", 
                    MessageBoxIcon.Error);
                Application.Exit();
                return;
            }
            initProcess(exePath, fileToOpen);
        }
        #endregion
        //启动进程
        Program.mainForm.systemTrayIcon.Icon = IconUtils.GetFileIcon(exePath, 
            false);
        if(iconName == null) {
            iconName = fileToOpen.Substring(fileToOpen.LastIndexOf("\\",
                StringComparison.Ordinal) + 1);
        }
        Program.mainForm.systemTrayIcon.Text = iconName;
        process.Start();
        launched = true;
        hookMinimizeEvent();
        process.WaitForExit();
        //进程结束，取消hook并退出
        winEventListener.Interrupt();
        Application.Exit();
    }

    private void initProcess(string fileName, string arg = null) {
        process = new Process {
            StartInfo = {
                //设置要启动的应用程序
                FileName = fileName,
                //是否使用操作系统shell启动
                UseShellExecute = false,
                //接受来自调用程序的输入信息
                RedirectStandardInput = false,
                //输出信息
                RedirectStandardOutput = false,
                //输出错误
                RedirectStandardError = false,
                //不显示程序窗口
                CreateNoWindow = false
            }
        };
        if(arg == null) return;
        process.StartInfo.Arguments = arg.Contains("\"") ? arg : $"\"{arg}\"";
    }

    private void hookMinimizeEvent() {
        //https://learn.microsoft.com/zh-cn/windows/win32/winauto/event-constants
        const uint EVENT_TYPE_ID = 0x0016;
        winEventListener = new Thread(() => {
            minimizeEventHookId = WinEventHookUtils.SetWinEventHook(
                EVENT_TYPE_ID, EVENT_TYPE_ID,
                IntPtr.Zero, winEventCallback,
                (uint) process.Id, 0, 0);
            Application.Run();
            WinEventHookUtils.UnhookWinEvent(minimizeEventHookId);
        });
        winEventListener.Start();
    }
    
    private void winEventCallback(IntPtr hWinEventHook, uint eventType, IntPtr hWnd, 
        int idObject, int idChild, uint dwEventThread, uint dwmsEventTime) {
        if(idObject != 0 || idChild != 0) return;
        //判断事件是否来自于主窗口
        if(process.MainWindowHandle.ToInt32() != hWnd.ToInt32()) return;
        //隐藏指定窗口
        WinEventHookUtils.ShowWindow(hWnd, 0);
        Program.mainForm.isWindowShow = false;
    }
}

}