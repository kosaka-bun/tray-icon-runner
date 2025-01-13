using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable InconsistentNaming

namespace TrayIconRunner.Util;

public static class AssociatedPrograms {

    private static readonly Dictionary<string, string> associatedProgramsMap = new();

    private static readonly string system32Path = Environment.GetFolderPath(Environment.SpecialFolder.System);

    private static readonly string[] directRunExtNames = [".bat", ".cmd"];

    static AssociatedPrograms() {
        associatedProgramsMap.Add(".bat", $"{system32Path}\\cmd.exe");
    }

    public static string get(string extName) {
        associatedProgramsMap.TryGetValue(extName, out string result);
        return result;
    }

    public static bool isDirectRunExtName(string extName) {
        return directRunExtNames.Contains(extName);
    }
}
