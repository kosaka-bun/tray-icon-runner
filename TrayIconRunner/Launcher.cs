using System;
using System.Diagnostics;
using System.IO;
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

    private Process process;

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
        process.WaitForExit();
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
        if(arg != null) {
            process.StartInfo.Arguments = $"\"{arg}\"";
        }
    }
}

}