using System;
using System.Collections.Generic;
using System.IO;
using System.Management;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.Win32;

// ReSharper disable InconsistentNaming

namespace TrayIconRunner.Util;

public static class Utils {
    
    public static void messageBox(string msg, MessageBoxIcon icon = MessageBoxIcon.Information) {
        MessageBox.Show(
            msg, Constant.APP_NAME, MessageBoxButtons.OK, icon,
            MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly
        );
    }

    // ReSharper disable once UnusedMember.Global
    public static Stream getResource(string fileName) {
        var resName = Constant.APP_NAME + ".Resources." + fileName;
        return Assembly.GetExecutingAssembly().GetManifestResourceStream(resName);
    }
    
    //返回值已经过trim
    public static string readFileToString(string filePath) {
        var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        var streamReader = new StreamReader(fileStream);
        var content = streamReader.ReadToEnd().Trim();
        streamReader.Close();
        fileStream.Close();
        return content;
    }

    public static string getAssociatedProgramPath(string extName) {
        if(extName == "") return null;
        var typeNamePath = "Software\\Microsoft\\Windows\\CurrentVersion" +
                           $"\\Explorer\\FileExts\\{extName}\\UserChoice";
        string extTypeKeyName;
        using(var extKey = Registry.CurrentUser.OpenSubKey(typeNamePath)) {
            var progId = extKey?.GetValue("ProgId");
            if(progId == null) return null;
            if(string.IsNullOrEmpty(progId.ToString())) return null;
            extTypeKeyName = $"{progId}\\shell\\open\\command";
        }
        string runCommand;
        using(var extTypeKey = Registry.ClassesRoot.OpenSubKey(extTypeKeyName)) {
            if(extTypeKey == null) return null;
            runCommand = extTypeKey.GetValue("").ToString();
        }
        var quote1Index = runCommand.IndexOf("\"", StringComparison.Ordinal);
        if(quote1Index == -1) {
            return runCommand.Substring(
                0, runCommand.IndexOf(" ", StringComparison.Ordinal)
            ).Trim();
        }
        var quote2Index = runCommand.IndexOf(".exe\"", StringComparison.Ordinal);
        if(quote2Index == -1) {
            quote2Index = runCommand.IndexOf(".EXE\"", StringComparison.Ordinal);
        }
        if(quote2Index == -1) {
            var result = runCommand.Substring(
                0, runCommand.IndexOf(" ", StringComparison.Ordinal)
            ).Trim();
            if(!result.EndsWith(".exe") && !result.EndsWith(".EXE")) {
                return null;
            }
        }
        quote2Index += 4;
        return runCommand.Substring(quote1Index + 1, quote2Index - 1);
    }

    public static string calcAbsolutePath(string basePath, string relativePath) {
        var path = basePath;
        path = path.Substring(0, path.LastIndexOf("\\", StringComparison.Ordinal));
        var parts = relativePath.Split('\\');
        foreach(var part in parts) {
            switch(part) {
                case ".":
                    continue;
                case "..":
                    path = path.Substring(0, path.LastIndexOf("\\", StringComparison.Ordinal));
                    continue;
                case "":
                    path = basePath.Substring(0, basePath.IndexOf("\\", StringComparison.Ordinal));
                    continue;
                default:
                    path += $"\\{part}";
                    continue;
            }
        }
        return path;
    }
    
    public static List<int> getSubProcessIdList(int pid) {
        var list = new List<int>();
        var searcher = new ManagementObjectSearcher(
            $"Select * From Win32_Process Where ParentProcessID = {pid}"
        );
        var moc = searcher.Get();
        foreach(var o in moc) {
            var mo = (ManagementObject) o;
            list.Add(Convert.ToInt32(mo["ProcessID"]));
        }
        return list;
    }
}
