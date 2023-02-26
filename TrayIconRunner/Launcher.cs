using System;
using System.Diagnostics;
using System.Windows.Forms;
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
        string exePath;
        if(content.Length <= 0) {
            exePath = filePath.Substring(0, filePath.Length - 
                suffix.Length) + ".exe";
            initProcess(exePath);
        } else {
            //读取指定扩展名的关联程序路径
            int pointIndex = filePath.LastIndexOf(".", 
                StringComparison.Ordinal);
            if(pointIndex == -1) {
                Utils.messageBox($"未找到 {filePath} 的关联程序", 
                    MessageBoxIcon.Error);
                Application.Exit();
                return;
            }
            string extName = filePath.Substring(pointIndex);
            exePath = Utils.getAssociatedProgramPath(extName);
            if(exePath == null) {
                Utils.messageBox($"未找到 {filePath} 的关联程序", 
                    MessageBoxIcon.Error);
                Application.Exit();
                return;
            }
            initProcess(exePath);
        }
        #endregion
        //启动进程
        
    }

    private void initProcess(string fileName) {
        process = new Process {
            StartInfo = {
                //设置要启动的应用程序
                FileName = fileName,
                //是否使用操作系统shell启动
                UseShellExecute = false,
                //接受来自调用程序的输入信息
                RedirectStandardInput = false,
                //输出信息
                RedirectStandardOutput = true,
                //输出错误
                RedirectStandardError = true,
                //不显示程序窗口
                CreateNoWindow = false
            }
        };
    }
}

}