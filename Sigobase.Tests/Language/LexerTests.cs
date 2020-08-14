using System;
using System.Collections.Generic;
using System.Linq;
using Sigobase.Language;
using Xunit;

namespace Sigobase.Tests.Language {
    public class LexerTests {
        [Theory]
        [InlineData("0", 0.0)]
        [InlineData("909", 909)]
        [InlineData("1.5", 1.5)]
        [InlineData("1E1", 1E1)]
        [InlineData("1.5E1", 1.5E1)]
        [InlineData("1.5E+1", 1.5E+1)]
        [InlineData("1.5E-1", 1.5E-1)]
        //[InlineData("Infinity", double.PositiveInfinity)]
        //[InlineData("NaN", double.NaN)]
        [InlineData("090.090e090", 090.090e090)]
        [InlineData("99.99e99", 99.99e99)]
        [InlineData("1e1000", double.PositiveInfinity)]
        public void NumberTest(string src, double value) {
            var lexer = new Lexer(src);
            var token = lexer.Read(null);
            Assert.Equal(value, token.Value);
        }

        [Theory]
        [InlineData("//\r")]
        [InlineData("//\n")]
        [InlineData("//\r\n")]
        [InlineData("//{[(/**/)]}\r")]
        [InlineData("/**/")]
        [InlineData("/*/*/")]
        [InlineData("/***/")]
        [InlineData("/*{[(\r\n /*)]}*/")]
        public void CommentTest(string src) {
            var lexer = new Lexer(src + 1);
            var token = lexer.Read(null);
            Assert.Equal(Kind.Number, token.Kind);
            Assert.Equal(src.Length, token.Start);
            Assert.Equal("1", token.Raw);
        }

        [Theory]
        [InlineData("//")]
        [InlineData("//\n")]
        [InlineData("//line")]
        [InlineData("//line\r")]
        [InlineData("/*block")]
        public void CommentEofTest(string src) {
            var lexer = new Lexer(src);
            var token = lexer.Read(null);
            Assert.Equal(Kind.Eof, token.Kind);
        }

        [Theory]
        [MemberData(nameof(SeparatorData))]
        public void SeparatorTest(string src, int start, int sep) {
            var lexer = new Lexer(src);
            var token = lexer.Read(null);
            Assert.Equal(src, token.Src);
            Assert.Equal(start, token.Start);
            Assert.Equal(sep, token.Separator);
        }

        public static IEnumerable<object[]> SeparatorData() {
            var dict = new Dictionary<int, List<string>> {
                {0, new List<string> {"", "/*block comment*/"}},
                {1, new List<string> {" ", "\t/**/", "  \t/*\n*/"}},
                {2, new List<string> {"\r", "\n", "//line\r", "//line\n"}},
                {3, new List<string> {"\r\t", "\n ", " //line\n"}}
            };

            foreach (var (sep, values) in dict) {
                foreach (var value in values) {
                    var src = value + 1;
                    var start = value.Length;
                    yield return new object[] {src, start, sep};
                }
            }
        }

        private static List<string> RawList(Kind kind) {
            switch (kind) {
                case Kind.Number:
                    return new List<string> {"0", "019"};
                case Kind.String:
                    return new List<string> {"'a'", "\"a\""};
                case Kind.Identifier:
                    return new List<string> {"_azAZ09", "true", "false", "Infinity", "NaN"};
                case Kind.Open:
                    return new List<string> {"{"};
                case Kind.Close:
                    return new List<string> {"}"};
                case Kind.Colon:
                    return new List<string> {":"};
                case Kind.Comma:
                    return new List<string> {","};
                case Kind.SemiColon:
                    return new List<string> {";"};
                case Kind.Or:
                    return new List<string> {"|"};
                case Kind.Div:
                    return new List<string> {"/"};
                case Kind.Question:
                    return new List<string> {"?"};
                case Kind.Eq:
                    return new List<string> {"="};
                case Kind.Unknown:
                    return new List<string> {"@"};
                case Kind.Eof:
                    return new List<string> { };
                case Kind.OpenBracket:
                    return new List<string> {"["};
                case Kind.CloseBracket:
                    return new List<string> {"]"};
                case Kind.OpenParens:
                    return new List<string> {"("};
                case Kind.CloseParens:
                    return new List<string> {")"};
                case Kind.Plus:
                    return new List<string> {"+"};
                case Kind.Minus:
                    return new List<string> {"-"};
                case Kind.Mul:
                    return new List<string> {"*"};
                case Kind.EqEq:
                    return new List<string> {"=="};
                case Kind.Not:
                    return new List<string> {"!"};
                case Kind.NotEq:
                    return new List<string> {"!="};
                default:
                    throw new Exception($"Give some raw strings of token {kind}");
            }
        }

        [Theory]
        [MemberData(nameof(TokenData))]
        public void TokenTest(Kind kind, string src, int start, int end) {
            var lexer = new Lexer(src);
            var token = lexer.Read(null);
            Assert.Equal(kind, token.Kind);
            Assert.Equal(start, token.Start);
            Assert.Equal(end, token.End);

            switch (kind) {
                case Kind.Number:
                    Assert.Equal(double.Parse(src.Substring(start, end - start)), token.Value);
                    break;
                case Kind.String:
                    Assert.Equal(src.Substring(start + 1, end - start - 2), token.Value);
                    break;
                default:
                    Assert.Null(token.Value);
                    break;
            }
        }

        public static IEnumerable<object[]> TokenData() {
            var kinds = Enum.GetValues(typeof(Kind))
                .Cast<Kind>()
                .Where(e => e != Kind.Eof)
                .ToList();

            var pre = " ";
            var sub = "{";

            foreach (var kind in kinds) {
                foreach (var raw in RawList(kind)) {
                    var src = pre + raw + sub;
                    var start = pre.Length;
                    var end = start + raw.Length;
                    yield return new object[] {kind, src, start, end};
                }
            }
        }
    }
}