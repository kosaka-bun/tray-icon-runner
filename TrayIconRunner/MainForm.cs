using System.Windows.Forms;
using TrayIconRunner.Util;

// ReSharper disable ConvertToAutoPropertyWhenPossible
// ReSharper disable MemberCanBeMadeStatic.Local
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable InconsistentNaming

namespace TrayIconRunner {

public partial class MainForm : Form {
    
    public NotifyIcon systemTrayIcon => notifyIcon1;

    public MainForm() {
        InitializeComponent();
        customInitializeComponent();
    }
    
    private void customInitializeComponent() {
        notifyIcon1.Text = Constant.APP_NAME;
        notifyIcon1.Icon = IconUtils.GetFileIcon(".exe", false);
    }

    private void notifyIcon1_MouseClick(object sender, MouseEventArgs e) {
        if(!Program.launcher.launched) return;
        if(e.Button != MouseButtons.Left) return;
        WinEventHookUtils.ShowWindow(Program.launcher.process.MainWindowHandle, 
            9);
        WinEventHookUtils.SetForegroundWindow(Program.launcher.process.MainWindowHandle);
    }
}

}