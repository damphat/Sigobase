﻿using System;
using System.Text;
using Sigobase.Generator.Utils;

namespace Sigobase.Generator.Lang {
    // TODO lexer.Reset(src);
    // TODO Token Read(Token reused);
    internal class Lexer {
        private const char Eof = char.MaxValue;
        private readonly string src;
        private int start;
        private int end;
        private char c;
        private char quote;
        private StringBuilder sb = new StringBuilder();
        private int separator;

        public Lexer(string src) {
            this.src = src;
            start = 0;
            end = 0;

            c = end < src.Length ? src[end] : Eof;
        }

        private void Next() {
            c = ++end < src.Length ? src[end] : Eof;
        }

        public Token Read() {
            separator = 0;
            while (true) {
                if (c == '\r' || c == '\n') {
                    separator |= 2;
                    Next();
                } else if (c == ' ' || c == '\t') {
                    separator |= 1;
                    Next();
                } else {
                    break;
                }
            }

            start = end;

            switch (c) {
                case '|': return CharToken(Kind.Or);
                case '{': return CharToken(Kind.Open);
                case '}': return CharToken(Kind.Close);
                case ',': return CharToken(Kind.Comma);
                case ';': return CharToken(Kind.SemiColon);
                case ':': return CharToken(Kind.Colon);
                case '=': return CharToken(Kind.Eq);
                case '?': return CharToken(Kind.Question);
                case '"':
                case '\'': return StringToken();
                default:
                    if (c >= '0' && c <= '9') {
                        return NumberToken();
                    }

                    if (c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z' || c == '_') {
                        return IdentifierToken();
                    }

                    if (c == Eof) {
                        return CreateToken(Kind.Eof);
                    }

                    return CharToken(Kind.Unknown);
            }
        }

        private Token CreateToken(Kind kind) {

            return new Token(kind, src, start, end, separator);
        }

        private Token CreateToken(Kind kind, object value) {
            return new Token(kind, src, start, end, separator, value);
        }

        private Token IdentifierToken() {
            Next();
            while (c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z' || c == '_' || c >= '0' && c <= '9') {
                Next();
            }

            return CreateToken(Kind.Identifier);
        }

        private Token CharToken(Kind kind) {
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
            while (c >= '0' && c <= '9') {
                Next();
            }

            return CreateToken(Kind.Number);
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
                    sb.Append(c); // warning
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