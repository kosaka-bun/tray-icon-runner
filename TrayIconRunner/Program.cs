using System.Windows.Forms;
using TrayIconRunner.Util;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable InconsistentNaming

namespace TrayIconRunner {

internal static class Program {

    public static MainForm mainForm;

    public static Launcher launcher;

    internal static void Main(string[] args) {
        if(args.Length < 1) {
            Utils.messageBox("没有提供要打开的文件", MessageBoxIcon.Error);
            return;
        }
        string inputFilePath = args[0];
        mainForm = new MainForm();
        launcher = new Launcher(inputFilePath);
        launcher.launch();
        using(mainForm) {
            Application.Run();
        }
    }
}

}