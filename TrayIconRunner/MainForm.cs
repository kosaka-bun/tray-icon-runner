using System;
using System.Threading;
using System.Windows.Forms;
using TrayIconRunner.Util;

// ReSharper disable ConvertToAutoPropertyWhenPossible
// ReSharper disable MemberCanBeMadeStatic.Local
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable InconsistentNaming

namespace TrayIconRunner {

public partial class MainForm : Form {
    
    public MenuItem appNameMenuItem;
    
    public NotifyIcon systemTrayIcon => notifyIcon1;

    public MainForm() {
        InitializeComponent();
        customInitializeComponent();
        init();
    }
    
    private void customInitializeComponent() {
        notifyIcon1.Text = Constant.APP_NAME;
        notifyIcon1.Icon = IconUtils.GetFileIcon(".exe", false);
    }

    private void init() {
        initTrayIcon();
    }
    
    private void initTrayIcon() {
        notifyIcon1.ContextMenu = new ContextMenu();
        Menu.MenuItemCollection menuItems = notifyIcon1.ContextMenu.MenuItems;
        //添加托盘图标菜单项
        appNameMenuItem = new MenuItem(Constant.APP_NAME) {
            Enabled = false
        };
        menuItems.Add(appNameMenuItem);
        menuItems.Add(new MenuItem("-"));
        var exitMenu = new MenuItem("退出");
        exitMenu.Click += (sender, e) => {
            DialogResult result = MessageBox.Show("确定退出吗？", "退出",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if(result != DialogResult.OK) return;
            new Thread(doExit).Start();
        };
        menuItems.Add(exitMenu);
    }
    
    private void doExit() {
        try {
            onExit();
        } catch(Exception) {
            DialogResult result = MessageBox.Show("退出时出现了异常，" +
                "是否强制退出？", "退出", MessageBoxButtons.OKCancel, 
                MessageBoxIcon.Question);
            if(result != DialogResult.OK) return;
        }
        Application.Exit();
    }
    
    private void onExit() {
        // Program.launcher.stopProcess();
    }

    private void notifyIcon1_MouseClick(object sender, MouseEventArgs e) {
        // throw new System.NotImplementedException();
    }
}

}