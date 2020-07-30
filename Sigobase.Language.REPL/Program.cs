using System;
using Sigobase.Database;
using Sigobase.Language.Lang;

namespace Sigobase.Language.REPL {
    internal class Program {
        private static void Main(string[] args) {
            while (true) {
                Console.Write("sigo>");
                var src = Console.ReadLine();
                var parser = new Parser(src);
                try {
                    Console.WriteLine(parser.Parse().ToString(2));
                } catch (Exception e) {
                    Console.Error.WriteLine(e.Message);
                    
                }
            }
        }
    }
}