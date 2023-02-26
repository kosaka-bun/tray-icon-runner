using System.IO;
using System.Reflection;
using System.Windows.Forms;

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
}

}