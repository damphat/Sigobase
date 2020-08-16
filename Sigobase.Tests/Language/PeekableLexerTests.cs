using System;
using Sigobase.Language;
using Xunit;

namespace Sigobase.Tests.Language {
    public class PeekableLexerTests {
        private readonly PeekableLexer lexer = new PeekableLexer("0 1 2 3 4 5", -1, 2);

        [Fact]
        public void PropertiesTest() {
            SigoAssert.Equal(-1, lexer.Min);
            SigoAssert.Equal(2, lexer.Max);
            SigoAssert.Equal(0, lexer.Cursor);

            lexer.Move(2);
            SigoAssert.Equal(2, lexer.Cursor);
            SigoAssert.Equal("2", lexer.Peek(0).Raw);

            lexer.Move(2);
            SigoAssert.Equal(4, lexer.Cursor);
            SigoAssert.Equal("4", lexer.Peek(0).Raw);

            lexer.Peek(lexer.Max); // after peek max, you can Move min, but not min-1
            lexer.Move(-1);
            SigoAssert.Equal(3, lexer.Cursor);
            SigoAssert.Equal("3", lexer.Peek(0).Raw);

            SigoAssert.ThrowsAny<Exception>(() => lexer.Peek(-1));

            // after move(-1) you have limitation
            SigoAssert.ThrowsAny<Exception>(() => lexer.Move(-1));
        }

        [Fact]
        public void ItCanPeekInMinMax() {
            for (var i = 0; i <= 5; i++) {
                // delta out of range
                SigoAssert.ThrowsAny<ArgumentOutOfRangeException>(() => lexer.Peek(-2));
                SigoAssert.ThrowsAny<ArgumentOutOfRangeException>(() => lexer.Peek(3));

                for (var d = lexer.Max; d >= lexer.Min; d--) {
                    if (i + d < 0) {
                        // negative position
                        SigoAssert.ThrowsAny<Exception>(() => lexer.Peek(d));
                    } else if (i + d > 5) {
                        // multiple eofs
                        SigoAssert.Equal(Kind.Eof, lexer.Peek(d).Kind);
                    } else {
                        // return the token
                        SigoAssert.Equal((i + d).ToString(), lexer.Peek(d).Raw);
                    }
                }

                lexer.Move(1);
            }
        }

        [Fact]
        public void ItCanMove() {
            lexer.Move(5);
            SigoAssert.Equal("5", lexer.Peek(0).Raw);
        }
    }
}