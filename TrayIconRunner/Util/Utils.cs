using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.Win32;

// ReSharper disable InconsistentNaming

namespace TrayIconRunner.Util {

public static class Utils {
    
    public static void messageBox(string msg, MessageBoxIcon icon) {
        MessageBox.Show(msg, Constant.APP_NAME,
            MessageBoxButtons.OK, icon,
            MessageBoxDefaultButton.Button1, 
            MessageBoxOptions.DefaultDesktopOnly);
    }

    public static Stream getResource(string fileName) {
        string resName = Constant.APP_NAME + ".Resources." + fileName;
        return Assembly.GetExecutingAssembly().GetManifestResourceStream(resName);
    }
    
    //返回值已经过trim
    public static string readFileToString(string filePath) {
        var fileStream = new FileStream(filePath, FileMode.Open,
            FileAccess.Read);
        var streamReader = new StreamReader(fileStream);
        string content = streamReader.ReadToEnd().Trim();
        streamReader.Close();
        fileStream.Close();
        return content;
    }

    public static string getAssociatedProgramPath(string extName) {
        string extTypeKeyName;
        using(RegistryKey extKey = Registry.ClassesRoot.OpenSubKey(extName)) {
            if(extKey == null) return null;
            extTypeKeyName = extKey.GetValue("") + 
                "\\shell\\open\\command";
        }
        string runCommand;
        using(RegistryKey extTypeKey = Registry.ClassesRoot
                  .OpenSubKey(extTypeKeyName)) {
            if(extTypeKey == null) return null;
            runCommand = extTypeKey.GetValue("").ToString();
        }
        int quote1Index = runCommand.IndexOf("\"", StringComparison.Ordinal);
        if(quote1Index == -1) {
            return runCommand.Substring(0, runCommand.IndexOf(" ",
                StringComparison.Ordinal)).Trim();
        }
        int quote2Index = runCommand.IndexOf(".exe\"", StringComparison.Ordinal);
        if(quote2Index == -1) {
            quote2Index = runCommand.IndexOf(".EXE\"", StringComparison.Ordinal);
        }
        if(quote2Index == -1) {
            return runCommand.Substring(0, runCommand.IndexOf(" ",
                StringComparison.Ordinal)).Trim();
        }
        quote2Index += 4;
        return runCommand.Substring(quote1Index + 1, quote2Index - 1);
    }
}

}