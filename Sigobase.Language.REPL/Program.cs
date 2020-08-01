using System;
using Sigobase.Database;
using Sigobase.Utils;

namespace Sigobase.Language.REPL {
    internal class Program {
        private static void Main(string[] args) {
            var writer = Writer.Pretty;
            while (true) {
                Console.Write("sigo>");
                var src = Console.ReadLine();
                try {
                    var sigo = new Parser(src).Parse();
                    Console.WriteLine(sigo.ToString(writer));
                } catch (Exception e) {
                    Console.Error.WriteLine(e.Message);
                }
            }
        }
    }
}