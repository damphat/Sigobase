using System.Collections.Generic;
using Sigobase.Database;
using Sigobase.Language.Utils;

namespace Sigobase.Language {
    internal class SigoParserV2 : SigoParser {
        private ISigo context = Sigo.Create(0);
        private readonly PathStack path = new PathStack();

        private readonly PeekableLexer lexer;
        private Token t;

        public SigoParserV2(string src) {
            lexer = new PeekableLexer(src, 0, 1);
            t = lexer.Peek(0);
        }

        private void Next() {
            lexer.Move(1);
            t = lexer.Peek(0);
        }

        private ParserException Expect(string what) {
            return new ParserException($"{what} expected, found {t.Raw} at {t.Start}");
        }

        private bool Eof() {
            return t.Kind == Kind.Eof;
        }

        private void Require(Kind kind) {
            if (t.Kind != kind) {
                Next();
                throw Expect(kind.ToString());
            }

            Next();
        }

        private Kind ReadKind() {
            var ret = t.Kind;
            Next();
            return ret;
        }

        private object Eat(object v) {
            Next();
            return v;
        }

        public object Factor() {
            object Parens() {
                Next(); // Require(Kind.OpenBracket);
                var expr = Expr();
                Require(Kind.CloseParens);
                return expr;
            }

            switch (t.Kind) {
                case Kind.Minus:
                case Kind.Plus: return Operators.Unary(ReadKind(), Factor());
                case Kind.Number: return Eat(t.Value);
                case Kind.String: return Eat(t.Value);
                case Kind.Identifier:
                    switch (t.Raw) {
                        case "true": return Eat(true);
                        case "false": return Eat(false);
                        case "Infinity": return Eat(double.PositiveInfinity);
                        case "NaN": return Eat(double.NaN);
                        default: return Eat(t.Raw);
                    }
                case Kind.OpenParens: return Parens();
                case Kind.OpenBracket: return ParseArray();
                case Kind.Open: return ParseObject();
                default:
                    throw Expect("factor");
            }
        }

        private bool Sep() {
            if (t.Kind == Kind.Comma || t.Kind == Kind.SemiColon) {
                Next();
                return true;
            }

            return t.Separator > 0;
        }

        private object ParseArray() {
            bool Close() {
                if (t.Kind == Kind.CloseBracket) {
                    Next();
                    return true;
                }

                return false;
            }

            Next();
            var list = new List<object>();
            if (Close()) {
                return list;
            }

            while (true) {
                list.Add(Expr());
                if (Close()) {
                    return list;
                }

                if (Sep()) {
                    if (Close()) {
                        return list;
                    } else {
                        /*loop*/
                    }
                } else {
                    throw Expect("','");
                }
            }
        }

        private void ReadKey() {
            switch (t.Kind) {
                case Kind.String:
                    path.Add((string) Eat(t.Value));
                    break;
                case Kind.Identifier:
                    path.Add1((string) Eat(t.Raw));
                    break;
                case Kind.Number:
                    path.Add1(Operators.ToStr((double) Eat(t.Value)));
                    break;
                default:
                    throw Expect("key");
            }
        }

        private void ReadPath() {
            bool KeySep() {
                if (t.Kind == Kind.Div) {
                    Next();
                    return true;
                }

                return false;
            }

            path.Clear();
            ReadKey();
            while (KeySep()) {
                ReadKey();
            }
        }

        private ISigo ReadPathValue(ISigo obj) {
            ReadPath();
            Require(Kind.Colon);
            var value = Expr();
            return obj.SetN(path, Sigo.From(value), path.Start);
        }

        private object ParseObject() {
            ISigo ParseFlag() {
                var raw = t.Raw;
                var k1 = lexer.Peek(1).Kind;
                if (raw.Length > 0 && raw[0] >= '0' && raw[0] <= '7' && k1 != Kind.Colon && k1 != Kind.Div) {
                    Next();
                    return Sigo.Create(raw[0] - '0');
                }

                return null;
            }

            bool Close() {
                if (t.Kind == Kind.Close) {
                    Next();
                    return true;
                }

                return false;
            }

            Next();
            var obj = ParseFlag();
            if (obj == null) {
                obj = Sigo.Create(3);
                if (Close()) {
                    return obj;
                } else {
                    /*loop*/
                }
            } else {
                if (Sep()) {
                    if (Close()) {
                        return obj;
                    } else {
                        /*loop*/
                    }
                } else {
                    if (Close()) {
                        return obj;
                    } else {
                        throw Expect("','");
                    }
                }
            }

            path.Push();
            try {
                while (true) {
                    obj = ReadPathValue(obj);
                    if (Close()) {
                        return obj;
                    }

                    if (Sep()) {
                        if (Close()) {
                            return obj;
                        } else {
                            /*loop*/
                        }
                    } else {
                        throw Expect("','");
                    }
                }
            } finally {
                path.Pop();
            }
        }

        public object MultiplicativeExpr() {
            var factor = Factor();
            while (true) {
                switch (t.Kind) {
                    case Kind.Mul:
                    case Kind.Div:
                        factor = Operators.Binary(ReadKind(), factor, Factor());
                        break;
                    default:
                        return factor;
                }
            }
        }

        public object AdditiveExpr() {
            var tong = MultiplicativeExpr();
            while (true) {
                switch (t.Kind) {
                    case Kind.Plus:
                    case Kind.Minus:
                        tong = Operators.Binary(ReadKind(), tong, MultiplicativeExpr());
                        break;
                    default:
                        return tong;
                }
            }
        }

        public object Expr() {
            return AdditiveExpr();
        }

        private bool ExprSep() {
            if (t.Kind == Kind.SemiColon) {
                Next();
                return true;
            }

            return false;
        }

        private object Program() {
            object last = Sigo.Create(0);
            if (Eof()) {
                return last;
            }

            while (true) {
                last = Expr();
                if (Eof()) {
                    return last;
                }

                if (ExprSep()) {
                    if (Eof()) {
                        return last;
                    }

                    continue;
                }

                throw Expect("';'");
            }
        }

        public override ISigo Parse() {
            return Sigo.From(Program());
        }

        public override ISigo Parse(ISigo input, out ISigo output) {
            input = input ?? Sigo.Create(0);
            input.Freeze();
            var ret = Parse();
            output = input;
            return ret;
        }
    }
}