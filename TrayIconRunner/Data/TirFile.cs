using System.Diagnostics.CodeAnalysis;

namespace TrayIconRunner.Data;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class TirFile {
    
    public string name { get; set; }
    
    public string file { get; set; }
    
    public string arguments { get; set; }
    
    public string executor { get; set; }
}
