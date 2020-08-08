using System;
using System.Collections.Generic;
using Sigobase.Database;
using Sigobase.Language.Utils;

namespace Sigobase.Language {
    public class SigoParserV2 : SigoParser {
        private readonly PeekableLexer lexer;
        private Token t;

        public SigoParserV2(string src) {
            lexer = new PeekableLexer(src, 0, 1);
            t = lexer.Peek(0);
        }

        void Next() {
            lexer.Move(1);
            t = lexer.Peek(0);
        }

        private Exception Expect(string what) {
            return new Exception($"{what} expected");
        }

        bool Eof() {
            return t.Kind == Kind.Eof;
        }

        void Require(Kind kind) {
            if (t.Kind != kind) {
                Next();
                throw Expect(kind.ToString());
            }

            Next();
        }

        Kind ReadKind() {
            var ret = t.Kind;
            Next();
            return ret;
        }

        object Eat(object v) {
            Next();
            return v;
        }


        public object Hang() {
            object Parens() {
                Next(); // Require(Kind.OpenBracket);
                var expr = Expr();
                Require(Kind.CloseParens);
                return expr;
            }

            switch (t.Kind) {
                case Kind.Minus:
                case Kind.Plus: return Operators.Unary(ReadKind(), Hang());
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
                    throw Expect("hang");
            }
        }

        bool Sep() {
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
            if (Close()) return list;

            while (true) {
                list.Add(Expr());
                if (Close()) return list;
                if (Sep()) {
                    if (Close()) return list;
                    else { /*loop*/}
                } else {
                    throw Expect("','");
                }
            }
        }

        private string ReadKey() {
            switch (t.Kind) {
                case Kind.String: return (string)Eat(t.Value);
                case Kind.Identifier: return (string)Eat(t.Raw);
                case Kind.Number: return Operators.ToStr(Eat(t.Value));
                default:
                    throw Expect("key");
            }
        }
        private ISigo ReadKeyValue(ISigo obj) {
            var key = ReadKey();
            Require(Kind.Colon);
            var value = Expr();
            return obj.Set1(key, Sigo.From(value));
        }

        private object ParseObject() {
            ISigo ParseFlag() {
                var raw = t.Raw;
                var k1 = lexer.Peek(1).Kind;
                if (raw.Length > 0 && raw[0] >= '0' && raw[0] <= '7' && (k1 != Kind.Colon && k1 != Kind.Div)) {
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
                if (Close()) return obj;
                else {
                    /*loop*/
                }
            } else {
                if (Sep()) {
                    if (Close()) return obj;
                    else { /*loop*/ }
                } else {
                    if (Close()) return obj;
                    else {
                        throw Expect("','");
                    }
                }

            }

            while (true) {
                obj = ReadKeyValue(obj);
                if (Close()) return obj;
                if (Sep()) {
                    if (Close()) return obj;
                    else { /*loop*/}
                } else {
                    throw Expect("','");
                }

            }

        }

        public object Tich() {
            var hang = Hang();
            while (true) {
                switch (t.Kind) {
                    case Kind.Mul:
                    case Kind.Div:
                        hang = Operators.Binary(ReadKind(), hang, Hang());
                        break;
                    default:
                        return hang;
                }
            }
        }

        public object Tong() {
            var tong = Tich();
            while (true) {
                switch (t.Kind) {
                    case Kind.Plus:
                    case Kind.Minus:
                        tong = Operators.Binary(ReadKind(), tong, Tich());
                        break;
                    default:
                        return tong;
                }
            }
        }

        /// <summary>
        /// TODO
        /// expr : tong
        /// tong : tich ( ('+'|'-') tich)*
        /// tich : hang ( ('*' | '/') hang) *
        /// hang : ('+' | '-') hang
        ///      | '(' expr ')'
        ///      | number
        ///      | identifier
        /// </summary>
        public object Expr() {
            return Tong();
        }

        /// <summary>
        /// program: eof
        ///        | loop
        /// 
        /// loop   : expr eof
        ///        | expr ';' eof 
        ///        | expr ';' loop
        ///        | expr Expect(';')
        /// </summary>
        private object Program() {
            bool ExprSep() {
                if (t.Kind == Kind.SemiColon) {
                    Next();
                    return true;
                }

                return false;
            }

            object last = Sigo.Create(0);
            if (Eof()) return last;

            // loop
            while (true) {
                last = Expr();
                if (Eof()) return last;
                if (ExprSep()) {
                    if (Eof()) return last;
                    else {
                        /*loop*/
                    }
                } else {
                    throw Expect("';'");
                }
            }
        }

        public override ISigo Parse() {
            return Sigo.From(Program());
        }

        public override ISigo Parse(ISigo input, out ISigo output) {
            output = input ?? Sigo.Create(0);
            return Parse();
        }
    }
}