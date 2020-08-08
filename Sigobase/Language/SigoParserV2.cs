using System;
using System.Diagnostics.CodeAnalysis;
using Sigobase.Database;

namespace Sigobase.Language {
    public class SigoParserV2 : SigoParser {
        private readonly PeekableLexer lexer;
        private Token t;

        public SigoParserV2(string src) 
        {
            lexer = new PeekableLexer(src,0,1);
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
            if (t.Kind == Kind.Number) {
                var ret = t.Value;
                Next();
                return ret;
            }

            if (t.Kind == Kind.Identifier) {
                var ret = t.Raw;
                Next();
                return ret;
            }
            throw Expect("expression");
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
            bool SemiColon() {
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
                if (SemiColon()) {
                    if (Eof()) return last;
                    else { /*loop*/ }
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