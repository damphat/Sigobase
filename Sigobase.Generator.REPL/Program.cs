﻿using System;
using Sigobase.Database;
using Sigobase.Generator.Lang;
using Sigobase.Generator.Schemas;

namespace Sigobase.Generator.REPL {
    internal class Program {
        private static void PeekableLexerTest() {
            var lex = new PeekableLexer("0 1 2 3 4 5 6", -1, 2);
            while (true) {
                try {
                    var cmd = Console.ReadLine();
                    if (string.IsNullOrEmpty(cmd)) {
                        continue;
                    }

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

        private static void Main() {
            var example = @"
                age  = 20 | 40; 
                name = 'An' | 'Bao'; 
                user = {name: name, age?: age}
                user = {name, age?}
                all  = user | name | age | {}
                ";

            Console.WriteLine(@"schema examples:");
            Console.WriteLine(example);

            new Parser(example).Parse();

            while (true) {
                Console.Write('>');
                var src = Console.ReadLine();
                if (string.IsNullOrEmpty(src)) {
                    continue;
                }

                switch (src) {
                    case "?":
                    case "help": {
                        foreach (var e in Schema.SchemaDict) {
                            Console.WriteLine($"{e.Key} = {e.Value}");
                        }

                        continue;
                    }
                    case "exit":
                    case "quit":
                        return;
                    default: {
                        var parser = new Parser(src);

                        try {
                            var schema = parser.Parse();
                            Console.WriteLine($"About {schema.Count()} items in {schema}");
                            Console.WriteLine("--------------------");
                            var count = 0;
                            var values = schema.Values(Options.UniqueSorted);
                            foreach (var value in values) {
                                Console.WriteLine(value.ToString(0));
                                count++;
                            }

                            Console.WriteLine($"{count}/{schema.Count()} items listed");
                        } catch (Exception e) {
                            Console.Error.WriteLine(e);
                        }

                        break;
                    }
                }
            }
        }
    }
}