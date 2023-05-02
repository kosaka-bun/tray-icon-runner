using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json;
using TrayIconRunner.Data;
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

    private readonly List<IntPtr> minimizeEventHookIds = new List<IntPtr>();

    private Thread winEventListener;

    public Launcher(string filePath) {
        this.filePath = filePath;
    }

    public void launch() {
        //读取专有文件
        string content = Utils.readFileToString(filePath);
        //如果文件为空，则调用该文件所在目录下，与该文件同名的，后缀名为.exe的文件
        string exePath, extName, iconName, fileToOpen;
        #region
        if(content.Length <= 0) {
            extName = ".exe";
            fileToOpen = exePath = filePath.Substring(0, 
                filePath.Length - suffix.Length) + extName;
            if(!File.Exists(exePath)) {
                Utils.messageBox($"{exePath} 文件不存在", MessageBoxIcon.Error);
                Application.Exit();
                return;
            }
            iconName = fileToOpen.Substring(fileToOpen.LastIndexOf("\\",
                StringComparison.Ordinal) + 1);
        } else {
            //解析专有文件
            var tirFile = JsonConvert.DeserializeObject<TirFile>(content);
            iconName = tirFile.name?.Trim();
            fileToOpen = tirFile.file?.Trim();
            exePath = tirFile.executor?.Trim();
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
            extName = fileToOpen.Substring(pointIndex);
            if(exePath == null) {
                exePath = Utils.getAssociatedProgramPath(extName);
            }
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
            if(!File.Exists(exePath)) {
                Utils.messageBox($"{exePath} 文件不存在", MessageBoxIcon.Error);
                Application.Exit();
                return;
            }
        }
        #endregion
        if(extName.ToLower().Equals(".exe")) {
            initProcess(fileToOpen);
        } else if(AssociatedPrograms.isDirectRunExtName(extName.ToLower())) {
            initProcess(fileToOpen, directRun: true);
        } else {
            initProcess(exePath, fileToOpen);
        }
        //启动进程
        Program.mainForm.systemTrayIcon.Icon = IconUtils.GetFileIcon(extName, 
            false);
        Program.mainForm.systemTrayIcon.Text = iconName;
        process.Start();
        launched = true;
        hookMinimizeEvent(AssociatedPrograms
            .isDirectRunExtName(extName.ToLower()));
        process.WaitForExit();
        //进程结束，取消hook并退出
        winEventListener.Interrupt();
        Application.Exit();
    }

    private void initProcess(string fileName, string arg = null, 
        bool directRun = false) {
        process = new Process {
            StartInfo = {
                //设置要启动的应用程序
                FileName = fileName,
                //是否使用操作系统shell启动
                UseShellExecute = directRun,
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
        if(directRun || arg == null) return;
        process.StartInfo.Arguments = arg.Contains("\"") ? arg : $"\"{arg}\"";
    }

    private void hookMinimizeEvent(bool hookSubProcesses) {
        //https://learn.microsoft.com/zh-cn/windows/win32/winauto/event-constants
        const uint EVENT_TYPE_ID = 0x0016;
        void doHook(int pid) {
            minimizeEventHookIds.Add(WinEventHookUtils.SetWinEventHook(
                EVENT_TYPE_ID, EVENT_TYPE_ID,
                IntPtr.Zero, winEventCallback,
                (uint) pid, 0, 0
            ));
        }
        winEventListener = new Thread(() => {
            doHook(process.Id);
            if(hookSubProcesses) {
                foreach(int sPid in Utils.getSubProcessId(process.Id)) {
                    doHook(sPid);
                }
            }
            Application.Run();
            foreach(IntPtr hookIds in minimizeEventHookIds) {
                WinEventHookUtils.UnhookWinEvent(hookIds);
            }
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