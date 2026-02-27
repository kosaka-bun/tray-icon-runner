using System;
using System.Threading;
using System.Windows.Forms;
using TrayIconRunner.Util;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable InconsistentNaming

namespace TrayIconRunner;

internal static class Program {

    public static MainForm mainForm;

    public static Launcher launcher;

    internal static void Main(string[] args) {
        if(args.Length < 1) {
            Utils.messageBox("没有提供要打开的文件", MessageBoxIcon.Error);
            return;
        }
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        //捕获UI线程上的未经处理的异常
        Application.ThreadException += (_, e) => onException(e);
        //捕获非UI线程上的未经处理的异常
        AppDomain.CurrentDomain.UnhandledException += (_, e) => onException(e);
        string inputFilePath = args[0];
        mainForm = new MainForm();
        launcher = new Launcher(inputFilePath);
        new Thread(() => launcher.launch()).Start();
        using(mainForm) {
            Application.Run();
        }
    }

    private static void onException(EventArgs args) {
        Exception e = args switch {
            ThreadExceptionEventArgs a => a.Exception,
            UnhandledExceptionEventArgs a => a.ExceptionObject as Exception,
            _ => null
        };
        if(e == null) return;
        Utils.messageBox(e.ToString(), MessageBoxIcon.Error);
        Application.Exit();
    }
}
