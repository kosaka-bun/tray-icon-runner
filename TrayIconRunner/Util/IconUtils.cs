using System;
using System.Drawing;
using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming
// ReSharper disable UnassignedField.Global
// ReSharper disable ArrangeTypeMemberModifiers

namespace TrayIconRunner.Util {

public static class IconUtils {
    
    [DllImport("Shell32.dll")]
    private static extern int SHGetFileInfo(string pszPath, uint dwFileAttributes, 
        ref SHFILEINFO psfi, uint cbFileInfo, uint uFlags);

    /// <summary>
    /// 从文件扩展名得到文件关联图标
    /// </summary>
    /// <param name="fileName">文件名或文件扩展名</param>
    /// <param name="smallIcon">是否是获取小图标，否则是大图标</param>
    /// <returns>图标</returns>
    public static Icon GetFileIcon(string fileName, bool smallIcon) {
        //也可用自带的
        //Icon icon = Icon.ExtractAssociatedIcon(fileName);
        var fi = new SHFILEINFO();
        Icon ic = null;
        //SHGFI_ICON + SHGFI_USEFILEATTRIBUTES + SmallIcon   
        int iTotal = SHGetFileInfo(fileName, 100, ref fi, 
            0, (uint) (smallIcon ? 273 : 272));
        if(iTotal > 0) {
            ic = Icon.FromHandle(fi.hIcon);
        }
        return ic;
    }
}

public struct SHFILEINFO {
    
    public IntPtr hIcon;
    
    public int iIcon;
    
    public uint dwAttributes;
    
    public char szDisplayName;
    
    public char szTypeName;
}

}