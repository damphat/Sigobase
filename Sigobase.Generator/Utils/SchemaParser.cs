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

                        var value = ParseOr();
                        ret.Add(key, value, optional);

                        
                    } else {
                        ret.Add(key, new ReferenceSchema(key), optional);

                    }

                    // skip 1 comma or semicolon if it exists
                    if ((t.Kind == Kind.Comma) || (t.Kind == Kind.SemiColon)) {
                        Next();
                    }

                    continue;
                }

                // if(t.Kind == Eq) suggestion 'Did you mean ":" ?'
                throw new Exception($"key expected, found {t.Kind}: '{t.Raw}'");
            }
        }

        private List<ISigo> ParseFlags() {
            var flags = "01234567";
            if (t.Kind == Kind.Number) {
                flags = t.Raw;
                Next();
                if ((t.Kind == Kind.Comma) || (t.Kind == Kind.SemiColon)) {
                    Next();
                }
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
                    throw new Exception($"SigoSchema expected, found {t.Kind}: '{t.Raw}'");
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

                    if (t.Kind == Kind.SemiColon) {
                        Next();
                    }
                } else {
                    break;
                }
            }

            if (t.Kind == Kind.Eof) {
                return new NothingSchema();
            }

            var ret = ParseOr();
            if (t.Kind != Kind.Eof) {
                throw new Exception($"eof expected, found {t.Kind}: '{t.Raw}'");
            }

            return ret;
        }
    }
}