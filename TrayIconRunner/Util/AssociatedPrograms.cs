using System.Collections.Generic;
using System.Linq;

// ReSharper disable InconsistentNaming

namespace TrayIconRunner.Util {

public static class AssociatedPrograms {

    private static readonly Dictionary<string, string> associatedProgramsMap = 
        new Dictionary<string, string>();

    private static readonly string[] directRunExtNames = {
        ".bat", ".cmd"
    };

    static AssociatedPrograms() {
        associatedProgramsMap.Add(".bat", "cmd.exe");
    }

    public static string get(string extName) {
        return associatedProgramsMap[extName];
    }

    public static bool isDirectRunExtName(string extName) {
        return directRunExtNames.Contains(extName);
    }
}

}