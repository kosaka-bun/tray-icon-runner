using System;

namespace TrayIconRunner {

internal static class Program {

    internal static void Main(string[] args) {
        foreach(string s in args) {
            Console.WriteLine(s);
        }
    }
}

}