using NUnit.Framework;
using TrayIconRunner.Util;

// ReSharper disable InconsistentNaming

namespace TrayIconRunner.Test {

[TestFixture]
public class AllTest {

    [Test]
    public void test1() {
        const string pathFilePath = "../../file/test/path.txt";
        string path = Utils.readFileToString(pathFilePath);
        var args = new string[1];
        args[0] = path;
        Program.Main(args);
    }
}

}