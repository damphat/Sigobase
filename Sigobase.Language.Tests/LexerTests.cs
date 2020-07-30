using System;
using System.Collections.Generic;
using System.Linq;
using Sigobase.Language.Lang;
using Xunit;

namespace Sigobase.Language.Tests
{
    public class LexerTests
    {
        private static IEnumerable<Token> Split(string src) {
            var lexer = new Lexer(src);
            while (true) {
                var token = lexer.Read(null);
                if (token.Kind == Kind.Eof) {
                    yield break;
                }
                yield return token;
            }
        }

        private static T[] Ls<T>(params T[] arr) {
            return arr;
        }

        [Fact]
        public void It_raw() {
            var src = " 1 2 3 ";

            var actual = Split(src).Select(t => t.Raw);
            var expected = Ls("1", "2", "3");
            Assert.Equal(expected, actual);
        }
    }
}
