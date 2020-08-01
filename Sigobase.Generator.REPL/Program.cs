using Sigobase.Database;
using Sigobase.Utils;
using System;
using System.Collections.Generic;

namespace Sigobase.Generator.REPL {
    internal class Program {

        private static void Help() {
            Console.WriteLine("Current Schemas:");
            foreach (var e in SigoSchema.SchemaDict) {
                Console.WriteLine($"\t{e.Key}\t=\t{e.Value}");
            }

            Console.WriteLine("  syntax: <name> '=' <schemaDefinition>  // to define a schema");
            Console.WriteLine("  syntax: <schemaDefinition> | <schemaName> // to list all possible values");
            Console.WriteLine();
        }

        private static void GenerateAllPossibleValues(SigoSchema schema) {
            Console.WriteLine($"About {schema.Count()} items in {schema}");
            Console.WriteLine("--------------------");
            var count = 0;
            var values = schema.Generate(GenerateOptions.UniqueSorted);
            foreach (var value in values) {
                Console.WriteLine(value.ToString(Writer.Default));
                count++;
            }
            Console.WriteLine($"\t{count}/{schema.Count()} items listed");
        }

        private static void Main() {
            var statements = new List<string> {
                "    string = 'usd'|'eur'",
                "    number = 1|2|3",
                "    bool = true|false",
                "    account = {kind: string, amount?: number}"
            };

            // eval the initial statements
            foreach (var statement in statements) {
                SigoSchema.Parse(statement);
            }

            Help();

            while (true) {
                Console.Write("schema>");
                var src = Console.ReadLine();
                switch (src) {
                    case null:
                    case "":
                        continue;
                    case "?":
                    case "help": {
                        Help();
                        continue;
                    }
                    case "exit":
                    case "quit":
                        return;
                    default: {

                        try {
                            var schema = SigoSchema.Parse(src);
                            if(schema.Count() == 0) continue;

                           GenerateAllPossibleValues(schema);
                        } catch (Exception e) {
                            Console.Error.WriteLine(e.Message);
                        }

                        break;
                    }
                }
            }
        }
    }
}