using System;
using System.Collections.Generic;
using Sigobase.Database;

namespace Sigobase.Language.Lang {
    public class Parser {
        private readonly PeekableLexer lexer;
        private Token t;

        private void Next() {
            lexer.Move(1);
            t = lexer.Peek(0);
        }

        public Parser(string src) {
            lexer = new PeekableLexer(src, 0, 1); // Peek(0), Peek(1)
            t = lexer.Peek(0);
        }

        private ISigo ParseValue() {
            switch (t.Kind) {
                case Kind.Number: return ParseNumber();
                case Kind.String: return ParseString();
                case Kind.Open: return ParseObject();
                case Kind.Identifier: return ParseIdentifier();
                case Kind.Eof:
                    throw new Exception($"value expected, found eof");
                default:
                    throw new Exception($"value expected, found '{t.Raw}' at {t.Start}");
            }
        }

        private ISigo ParseIdentifier() {
            var raw = t.Raw;
            switch (raw) {
                case "true": return Sigo.From(true);
                case "false": return Sigo.From(false);
                case "NaN": return Sigo.From(double.NaN);
                case "Infinity": return Sigo.From(double.PositiveInfinity);
                default:
                    throw new Exception($"unexpected identifier '{raw}'");
            }
        }

        private string ReadKey() {
            string key;
            if (t.Kind == Kind.Identifier) {
                key = t.Raw;
                Next();
                return key;
            }
            if (t.Kind == Kind.Number || t.Kind == Kind.String) {
                key = t.Value.ToString();
                Next();
                return key;
            }

            return null;
        }

        private List<string> ReadKeys() {
            var key = ReadKey();
            if (key == null) return null;

            var keys = new List<string> {key};
            while (t.Kind == Kind.Div) {
                Next();
                key = ReadKey();
                if(key == null) throw new Exception($"key expected after '/' at {t.Start}");
                keys.Add(key);
            }

            return keys;
        }

        private ISigo ParseObject() {
            ISigo ret = Sigo.Create(3);
            Next();
            var k1 = lexer.Peek(1).Kind;
            if (t.Kind == Kind.Number && k1 != Kind.Colon && k1 != Kind.Div) {
                var flags = int.Parse(t.Raw);
                ret = Sigo.Create(flags);
                Next();
                if(t.Kind == Kind.Comma) Next();
            }

            while (true) {
                if (t.Kind == Kind.Close) {
                    Next();
                    return ret;
                }

                if (t.Kind == Kind.Eof) {
                    throw new Exception("unexpected end of stream");
                }

                var keys = ReadKeys();
                if (keys != null) {
                    if (t.Kind == Kind.Colon) {
                        Next();
                    } else {
                        throw new Exception($"':' expected at {t.Start}");
                    }

                    var value = ParseValue();
                    ret = ret.SetN(keys, value, 0);

                    if (t.Kind == Kind.Comma) {
                        Next();
                    }
                } else {
                    throw new Exception($"key expected at {t.Start}");
                }

            }
        }

        private ISigo ParseString() {
            var ret = t.Value;
            Next();
            return Sigo.From(ret);
        }

        private ISigo ParseNumber() {
            var ret = t.Value;
            Next();
            return Sigo.From(ret);
        }

        public ISigo Parse() {
            var ret = ParseValue();
            if(t.Kind == Kind.Eof)
                return ret;
            throw new Exception($"unexpected '{t.Raw}' at {t.Start}");
        }
    }
}