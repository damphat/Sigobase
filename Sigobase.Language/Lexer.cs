using System;
using System.Text;
using Sigobase.Language.Utils;

namespace Sigobase.Language {
    public class Lexer {
        private const char Eof = char.MaxValue;
        private readonly string src;
        private int start;
        private int end;
        private int separator;
        private Token token;

        private char c;
        private char quote;
        private readonly StringBuilder sb = new StringBuilder();

        public Lexer(string src) {
            this.src = src;
            start = 0;
            end = 0;

            c = end < src.Length ? src[end] : Eof;
        }

        private void Next() {
            c = ++end < src.Length ? src[end] : Eof;
        }

        private char Peek(int i) {
            i += end;
            return i < src.Length ? src[i] : Eof;
        }


        private void ScanLineComment() {
            Next();
            Next();
            while (c != '\r' && c != '\n' && c != Eof) {
                Next();
            }
        }

        private void ScanBlockComment() {
            Next();
            Next();

            // search for */ or eof
            while (true) {
                switch (c) {
                    case '*': {
                        Next();
                        if (c == '/') {
                            Next();
                            return; // OK
                        } else {
                            continue;   // goto */ | eof
                        }
                    }
                    case Eof:
                        return; // unexpected eof
                    default:
                        Next();
                        break;
                }
            }
        }

        private void ScanSeparator() {
            separator = 0;
            while (true) {
                switch (c) {
                    case ' ':
                    case '\t':
                        separator |= 1;
                        Next();
                        break;
                    case '\r':
                    case '\n':
                        separator |= 2;
                        Next();
                        break;

                    case '/': {
                        switch (Peek(1)) {
                            case '/':
                                ScanLineComment();
                                break;
                            case '*':
                                ScanBlockComment();
                                break;
                            default:
                                return; //  slash 
                        }

                        break;
                    }
                    default:
                        return;  // not space & not slash
                }
            }
        }

        public Token Read(Token token) {
            this.token = token;
            ScanSeparator();

            start = end;

            switch (c) {
                case '+': return CharToken(Kind.Plus);
                case '-': return CharToken(Kind.Minus);
                case '*': return CharToken(Kind.Mul);
                case '/': return CharToken(Kind.Div);
                case '|': return CharToken(Kind.Or);

                case '{': return CharToken(Kind.Open);
                case '}': return CharToken(Kind.Close);
                case '[': return CharToken(Kind.OpenBracket);
                case ']': return CharToken(Kind.CloseBracket);
                case '(': return CharToken(Kind.OpenParens);
                case ')': return CharToken(Kind.CloseParens);
                case ',': return CharToken(Kind.Comma);
                case ';': return CharToken(Kind.SemiColon);
                case ':': return CharToken(Kind.Colon);
                case '?': return CharToken(Kind.Question);

                case '!': {
                    switch (Peek(1)) {
                        case '=': return TwoCharToken(Kind.NotEq);
                    }

                    return CharToken(Kind.Not);
                }
                case '=': {
                    switch (Peek(1)) {
                        case '=': return TwoCharToken(Kind.EqEq);
                    }
                    return CharToken(Kind.Eq);
                }
                case '"':
                case '\'': return StringToken();
                default:
                    if (Chars.Digit(c)) {
                        return NumberToken();
                    }

                    if (Chars.IdentifierStart(c)) {
                        return IdentifierToken();
                    }

                    if (c == Eof) {
                        return CreateToken(Kind.Eof);
                    }

                    return CharToken(Kind.Unknown);
            }
        }

        private Token CreateToken(Kind kind, object value = null) {
            if (token == null) {
                return new Token(kind, src, start, end, separator, value);
            } else {
                token.Reset(kind, src, start, end, separator, value);
                return token;
            }
        }

        private Token IdentifierToken() {
            Next();
            while (Chars.IdentifierPart(c)) {
                Next();
            }

            return CreateToken(Kind.Identifier);
        }

        private Token CharToken(Kind kind) {
            Next();
            return CreateToken(kind);
        }

        private Token TwoCharToken(Kind kind) {
            Next();
            Next();
            return CreateToken(kind);
        }

        // TODO parse fraction & exponent
        // TODO parse sign, +1 -1
        // TODO parse -Infinity, NaN
        // TODO parse -1E1000 => -Infinity
        // TODO parse 1_000
        // TODO parse 0xffff
        private Token NumberToken() {
            Next();
            while (Chars.Digit(c)) {
                Next();
            }

            var value = double.Parse(src.Substring(start, end - start));

            return CreateToken(Kind.Number, value);
        }

        private void ScanStringEscape() {
            Next();
            switch (c) {
                case Eof:
                    throw new Exception("UnterminatedStringLiteral");
                case '\r':
                    Next();
                    if (c == '\n') {
                        Next();
                    }

                    break;
                case '\n':
                    Next();
                    break;

                case '\\':
                case '"':
                case '\'':
                    sb.Append(c);
                    Next();
                    break;

                case 'b':
                    sb.Append('\b');
                    Next();
                    break;

                case 'f':
                    sb.Append('\f');
                    Next();
                    break;

                case 'r':
                    sb.Append('\r');
                    Next();
                    break;

                case 'n':
                    sb.Append('\n');
                    Next();
                    break;

                case 't':
                    sb.Append('\t');
                    Next();
                    break;

                case 'u':
                    Next();
                    var u = 0;
                    var i = 0;
                    for (; i < 4; i++) {
                        var h = Chars.ToHex(c);
                        if (h >= 0) {
                            Next();
                            u = u * 16 + h;
                        } else {
                            break;
                        }
                    }

                    if (i == 4) {
                        sb.Append((char) u);
                    } else {
                        throw new Exception("HexadecimalDigitExpected");
                    }

                    break;
                default: // /c
                    sb.Append(c); // warning unknown escape
                    Next();
                    break;
            } // switch (the letter after \)
        }

        private Token StringToken() {
            quote = c;
            sb.Clear();

            Next();

            while (true) {
                switch (c) {
                    case Eof:
                    case '\r':
                    case '\n':
                        throw new Exception("UnterminatedStringLiteral");
                    case '"':
                    case '\'':
                        if (c == quote) {
                            Next();
                            return CreateToken(Kind.String, sb.ToString()); // OK
                        } else {
                            sb.Append(c);
                            Next();
                            continue;
                        }
                    case '\\':
                        ScanStringEscape();
                        break;

                    default:
                        sb.Append(c);
                        Next();
                        break;
                } // switch(c)
            } // while
        } // fn
    }
}