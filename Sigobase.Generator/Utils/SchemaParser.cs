using System;
using System.Collections.Generic;
using System.Linq;
using Sigobase.Database;
using Sigobase.Generator.Schemas;
using Sigobase.Language;

namespace Sigobase.Generator.Utils {
    internal class SchemaParser {
        private readonly PeekableLexer lexer;
        private Token t;

        private void Next() {
            lexer.Move(1);
            t = lexer.Peek(0);
        }

        public SchemaParser(string src) {
            lexer = new PeekableLexer(src, 0, 1); // Peek(0), Peek(1)
            t = lexer.Peek(0);
        }

        private string Found() {
            return t.Kind != Kind.Eof ? $"'{t.Raw}'" : "eof";
        }

        private string Expected(string thing) {
            return $"{thing} expected, found {Found()} at {t.Start}";
        }

        private string ReadKey() {
            string key;
            switch (t.Kind) {
                case Kind.Identifier:
                    key = t.Raw;
                    Next();
                    return key;
                case Kind.Number:
                    key = t.Value.ToString();
                    Next();
                    return key;
                case Kind.String:
                    key = (string) t.Value;
                    Next();
                    return key;
                default:
                    throw new Exception(Expected("key"));
            }
        }

        private bool ReadKind(Kind kind) {
            if (t.Kind == kind) {
                Next();
                return true;
            }

            return false;
        }

        private SigoSchema ParseObject() {
            Next();
            var ret = new ObjectSchema {
                Flags = ParseFlags()
            };

            while (true) {
                if (t.Kind == Kind.Close) {
                    Next();
                    return ret;
                }

                var key = ReadKey();
                var optional = ReadKind(Kind.Question);
                var hasColon = ReadKind(Kind.Colon);

                SigoSchema value;

                if (hasColon) {
                    value = ParseOr();
                } else if (SigoSchema.Context.ContainsKey(key)) {
                    value = SigoSchema.Context[key];
                } else {
                    throw new Exception(Expected("':'"));
                }

                ret.Add(key, value, optional);

                // skip 1 comma or semicolon if it exists
                if (t.Kind == Kind.Comma || t.Kind == Kind.SemiColon) {
                    Next();
                }
            }
        }

        private List<ISigo> ParseFlags() {
            var flags = "3";

            bool IsFlag() {
                if (t.Kind != Kind.Number) {
                    return false;
                }

                var t1 = lexer.Peek(1).Kind;
                if (t1 == Kind.Colon) {
                    return false;
                }

                return true;
            }

            if (IsFlag()) {
                flags = t.Raw;
                Next();
                if (t.Kind == Kind.Comma || t.Kind == Kind.SemiColon) {
                    Next();
                }
            } else if (t.Kind == Kind.Question) {
                Next();
                flags = "01234567";
            }

            return flags.Select(c => Sigo.Create(c - '0')).ToList();
        }

        private SigoSchema ParseNumber() {
            var number = double.Parse(t.Raw);
            Next();
            return new ValueSchema(number);
        }

        private SigoSchema ParseString() {
            var str = t.Value;
            Next();
            return new ValueSchema(str);
        }

        private SigoSchema ParseIdentifier() {
            switch (t.Raw) {
                case "true": {
                    var ret = new ValueSchema(true);
                    Next();
                    return ret;
                }
                case "false": {
                    var ret = new ValueSchema(false);
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
                    return new ReferenceSchema(key);
            }
        }

        private SigoSchema ParseSingle() {
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
                    throw new Exception(Expected("value"));
            }
        }

        private SigoSchema ParseOr() {
            var item = ParseSingle();
            if (t.Kind != Kind.Or) {
                return item;
            }

            var items = new List<SigoSchema> {item};

            // ('|' single)+
            while (true) {
                Next(); // '|'
                item = ParseSingle();
                items.Add(item);
                if (t.Kind != Kind.Or) {
                    return new ListSchema(items);
                }
            }
        }

        public SigoSchema Parse() {
            while (true) {
                if (t.Kind == Kind.Identifier && lexer.Peek(1).Kind == Kind.Eq) {
                    var key = t.Raw;
                    Next();
                    Next();
                    var value = ParseOr();
                    SigoSchema.SetType(key, value);

                    ReadKind(Kind.SemiColon);
                } else {
                    break;
                }
            }

            if (t.Kind == Kind.Eof) {
                return new NothingSchema();
            }

            var ret = ParseOr();
            ReadKind(Kind.SemiColon);

            if (t.Kind != Kind.Eof) {
                throw new Exception($"unexpected token, found {Found()} at {t.Start}");
            }

            return ret;
        }
    }
}