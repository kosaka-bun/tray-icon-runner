using System.Collections.Generic;

// ReSharper disable InconsistentNaming

namespace TrayIconRunner.Util {

public static class AssociatedPrograms {

    private static readonly Dictionary<string, string> map = new 
        Dictionary<string, string>();

    static AssociatedPrograms() {
        map.Add(".bat", "cmd.exe");
    }

    public static string get(string extName) {
        return map[extName];
    }
}

}