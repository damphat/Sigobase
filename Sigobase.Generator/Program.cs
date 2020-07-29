using System;
using System.Linq;
using Sigobase.Database;
using Sigobase.Generator.Lang;
using Sigobase.Generator.Utils;

namespace Sigobase.Generator {
    class Program {
        static void PeekableLexerTest() {
            var lex = new PeekableLexer("0 1 2 3 4 5 6", -1, 2);
            while (true) {
                try {
                    var cmd = Console.ReadLine();
                    if(string.IsNullOrEmpty(cmd)) continue;
                    
                    switch (cmd[0]) {
                        case 'm':
                            lex.Move(int.Parse(cmd.Substring(1)));
                            break;
                        case 'p':
                            lex.Peek(int.Parse(cmd.Substring(1)));
                            break;

                    }
                    Console.WriteLine(lex);

                } catch (Exception e) {
                    Console.Error.WriteLine(e.Message);
                }
            }
        }

        static void Main(string[] args) {
           
            Console.WriteLine(@"schema examples:");
            Console.WriteLine(@"'An' | 'Bao'");
            Console.WriteLine(@"'unknown' | {0} | {7, lon: 0|1,  lat: 0|1}");

            while (true) {
                Console.Write('>');
                var src = Console.ReadLine();
                if (string.IsNullOrEmpty(src)) continue;

                var parser = new Parser(src);

                try {
                    var schema = parser.Parse();
                    Console.WriteLine($"About {schema.Count()} items in {schema}");
                    Console.WriteLine("--------------------");
                    var count = 0;
                    var values = schema.Values(true).ToList();
                    values.Sort(new SigoComparer());
                    foreach (var value in values) {
                        Console.WriteLine(value.ToString(0));
                        count++;
                    }

                    Console.WriteLine($"{count}/{schema.Count()} items listed");
                } catch (Exception e) {
                    Console.Error.WriteLine(e);
                }
            }
        }
    }
}