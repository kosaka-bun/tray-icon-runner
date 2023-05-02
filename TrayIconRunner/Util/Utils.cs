using System;
using System.Collections.Generic;
using System.IO;
using System.Management;
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
        string typeNamePath = "Software\\Microsoft\\Windows\\CurrentVersion" +
                              $"\\Explorer\\FileExts\\{extName}\\UserChoice";
        string extTypeKeyName;
        using(RegistryKey extKey = Registry.CurrentUser.OpenSubKey(typeNamePath)) {
            object progId = extKey?.GetValue("ProgId");
            if(progId == null) return null;
            if(string.IsNullOrEmpty(progId.ToString())) return null;
            extTypeKeyName = $"{progId}\\shell\\open\\command";
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
            string result = runCommand.Substring(0, 
                runCommand.IndexOf(" ", StringComparison.Ordinal)
            ).Trim();
            if(!result.EndsWith(".exe") && !result.EndsWith(".EXE")) {
                return null;
            }
        }
        quote2Index += 4;
        return runCommand.Substring(quote1Index + 1, quote2Index - 1);
    }

    public static string calcAbsolutePath(string basePath, string relativePath) {
        string path = basePath;
        path = path.Substring(0, path.LastIndexOf("\\", 
            StringComparison.Ordinal));
        string[] parts = relativePath.Split('\\');
        foreach(string part in parts) {
            switch(part) {
                case ".":
                    continue;
                case "..":
                    path = path.Substring(0, path.LastIndexOf(
                        "\\", StringComparison.Ordinal));
                    continue;
                default:
                    path += $"\\{part}";
                    continue;
            }
        }
        return path;
    }
    
    public static List<int> getSubProcessId(int pid) {
        var list = new List<int>();
        var searcher = new ManagementObjectSearcher("Select * From " +
            $"Win32_Process Where ParentProcessID = {pid}");
        ManagementObjectCollection moc = searcher.Get();
        foreach(ManagementBaseObject o in moc) {
            var mo = (ManagementObject) o;
            list.Add(Convert.ToInt32(mo["ProcessID"]));
        }
        return list;
    }
}

}