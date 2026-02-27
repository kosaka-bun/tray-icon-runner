using System;
using NUnit.Framework;
using TrayIconRunner.Util;

// ReSharper disable MemberCanBeMadeStatic.Local
// ReSharper disable InconsistentNaming

namespace TrayIconRunner.Test;

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
        string[] args = [".zip", ".doc", ".xlsx", ".mp3"];
        foreach(string s in args) {
            Console.WriteLine(Utils.getAssociatedProgramPath(s));
        }
    }

    [Test]
    public void calcAbsolutePathTest() {
        string[] arg0 = [
            "C:\\dir1\\dir2\\",
            "C:\\dir1\\dir2\\1.txt",
            "C:\\dir1\\dir2",
            "C:\\dir1\\dir2\\",
            "C:\\dir1\\dir2\\1",
            "C:\\dir1\\dir2\\1",
            "C:\\dir1\\dir2\\1",
            "C:\\dir1\\dir2\\1"
        ];
        string[] arg1 = [
            "2.txt",
            "2.txt",
            "2.txt",
            "dir3\\..\\2.txt",
            ".\\3.txt",
            "..\\dir3\\3.txt",
            "\\abc",
            "abc\\\\def"
        ];
        for(var i = 0; i < arg0.Length; i++) {
            Console.WriteLine(Utils.calcAbsolutePath(arg0[i], arg1[i]));
        }
    }
}
