using System;
using Sigobase.Database;

namespace Sigobase.REPL {
    internal class Program {
        private static void Main(string[] args) {
            var user = Sigo.Create(1,
                "name", Sigo.Create(1,
                    "first", "Phat",
                    "last", "Dam"),
                "age", 40);

            Console.WriteLine(user);
            Console.Read();
        }
    }
}