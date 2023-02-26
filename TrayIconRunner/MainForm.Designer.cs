using System.ComponentModel;

namespace TrayIconRunner {

partial class MainForm {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
        if(disposing && (components != null)) {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
        this.components = new System.ComponentModel.Container();
        this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
        this.SuspendLayout();
        // 
        // notifyIcon1
        // 
        this.notifyIcon1.Text = "notifyIcon1";
        this.notifyIcon1.Visible = true;
        this.notifyIcon1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseClick);
        // 
        // MainForm
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(800, 450);
        this.Name = "MainForm";
        this.Text = "MainForm";
        this.ResumeLayout(false);
    }

    private System.Windows.Forms.NotifyIcon notifyIcon1;

    #endregion

}

}