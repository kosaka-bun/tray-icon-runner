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
        Application.ThreadException += (_, e) => onException(e.Exception);
        string inputFilePath = args[0];
        mainForm = new MainForm();
        launcher = new Launcher(inputFilePath);
        new Thread(() => {
            try {
                launcher.launch();
            } catch(Exception e) {
                onException(e);
            }
        }).Start();
        using(mainForm) {
            Application.Run();
        }
    }

    private static void onException(Exception e) {
        Utils.messageBox(e.ToString(), MessageBoxIcon.Error);
        Application.Exit();
    }
}
