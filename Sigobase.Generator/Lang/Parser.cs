using System;
using System.Collections.Generic;
using System.Linq;
using Sigobase.Database;
using Sigobase.Generator.Schemas;

namespace Sigobase.Generator.Lang {
    class Parser {
        private PeekableLexer lexer;
        private Token t;

        private void Next() {
            lexer.Move(1);
            t = lexer.Peek(0);
        }

        public Parser(string src) {
            this.lexer = new PeekableLexer(src, 0, 1);
            t = lexer.Peek(0);
        }

        public Schema ParseObject() {
            Next();
            var ret = new ObjSchema {
                Flags = ParseFlags()
            };

            while (true) {
                if (t.Kind == Kind.Close) {
                    Next();
                    return ret;
                }

                if (t.Kind == Kind.Identifier) {
                    var key = t.Raw;
                    Next();
                    var optional = false;
                    if (t.Kind == Kind.Question) {
                        optional = true;
                        Next();
                    }

                    if (t.Kind == Kind.Colon) {
                        Next();
                    } else {
                        throw new Exception("colon expected");
                    }

                    var value = ParseOr();
                    ret.Add(key, value, optional);

                    // skip 1 comma or semicolon if it exists
                    if (t.Kind == Kind.Comma | t.Kind == Kind.SemiColon) {
                        Next();
                    }

                    continue;
                }

                throw new Exception("key expected");
            }
        }

        private List<ISigo> ParseFlags() {
            var flags = "01234567";
            if (t.Kind == Kind.Number) {
                flags = t.Raw;
                Next();
                if (t.Kind == Kind.Comma | t.Kind == Kind.SemiColon) {
                    Next();
                }
            }

            return flags.Select(c => Sigo.Create(c - '0')).ToList();
        }

        private Schema ParseNumber() {
            var number = double.Parse(t.Raw);
            Next();
            return new SingleValueSchema(number);
        }

        private Schema ParseString() {
            var str = t.Value;
            Next();
            return new SingleValueSchema(str);
        }

        private static Dictionary<string, Schema> cache = new Dictionary<string, Schema>();

        private Schema ParseIdentifier() {
            switch (t.Raw) {
                case "true": {
                    var ret = new SingleValueSchema(true);
                    Next();
                    return ret;
                }
                case "false": {
                    var ret = new SingleValueSchema(false);
                    Next();
                    return ret;
                }
                case "null":
                case "NaN":
                case "Infinity":
                    throw new Exception($"'{t.Raw}' is not supported");
                default:
                    var key = t.Raw;
                    Next();
                    if (cache.TryGetValue(key, out var schema)) {
                        return schema;
                    } else {
                        throw new Exception($"schema '{key}' is not found");
                    }
            }
        }

        private Schema ParseSingle() {
            switch (t.Kind) {
                case Kind.Number:
                    return ParseNumber();
                case Kind.String:
                    return ParseString();
                case Kind.Open:
                    return ParseObject();
                case Kind.Identifier:
                    return ParseIdentifier();
                default:
                    throw new Exception("Schema expected");
            }
        }

        private Schema ParseOr() {
            var item = ParseSingle();
            if (t.Kind != Kind.Or) return item;

            var orSchema = new OrSchema();
            orSchema.Add(item);

            // ('|' single)+
            while (true) {
                Next(); // '|'
                item = ParseSingle();
                orSchema.Add(item);
                if (t.Kind != Kind.Or) {
                    return orSchema;
                }
            }
        }

        public Schema Parse() {
            while (true) {
                if (t.Kind == Kind.Identifier && lexer.Peek(1).Kind == Kind.Eq) {
                    var key = t.Raw;
                    Next();
                    Next();
                    var value = ParseOr();
                    cache[key] = value;

                    if (t.Kind == Kind.SemiColon) {
                        Next();
                    }

                } else {
                    break;
                }
            }

            if (t.Kind == Kind.Eof) {
                return new OrSchema();
            }

            var ret = ParseOr();
            if (t.Kind != Kind.Eof) {
                throw new Exception("eof expected");
            }

            return ret;
        }
    }
}