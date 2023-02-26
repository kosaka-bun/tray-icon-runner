using System;
using NUnit.Framework;
using TrayIconRunner.Util;

// ReSharper disable MemberCanBeMadeStatic.Local
// ReSharper disable InconsistentNaming

namespace TrayIconRunner.Test {

[TestFixture]
public class AllTest {

    private void run(string pathFilePath) {
        string path = Utils.readFileToString(pathFilePath);
        var args = new string[1];
        args[0] = path;
        Program.Main(args);
    }

    [Test]
    public void test1() {
        run("../../file/test/path.txt");
    }

    [Test]
    public void getAssociatedProgramPathTest() {
        string[] args = { ".zip", ".doc", ".xlsx", ".mp3" };
        foreach(string s in args) {
            Console.WriteLine(Utils.getAssociatedProgramPath(s));
        }
    }
}

}