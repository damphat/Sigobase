using System;
using Sigobase.Database;
using Sigobase.Utils;

namespace Sigobase.REPL {
    internal class Program {
        private static readonly Writer DefaultWriter = Writer.Pretty;

        private static void Main(string[] args) {
            Console.WriteLine("Welcome to sigo REPL");
            while (true) {
                Console.Write("sigo> ");
                var src = Console.ReadLine();
                if (src == "exit") {
                    return;
                }
                if (src == "") continue;
                if (src == "cls" || src == "clear") {
                    Console.Clear();
                    continue;
                }

                try {
                    Console.WriteLine(Sigo.Parse(src).ToString(DefaultWriter));
                } catch (Exception e) {
                    Console.Error.WriteLine(e.Message);
                }
            }
        }
    }
}