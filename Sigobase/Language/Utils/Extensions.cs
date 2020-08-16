using System;

namespace Sigobase.Language.Utils {
    public static class Extensions {
        public static string ToStringEx(this TokenErrorKind kind) {
            switch (kind) {
                case TokenErrorKind.None:
                    return "None";
                case TokenErrorKind.UnterminatedStringLiteral:
                    return "Unterminated string literal";
                case TokenErrorKind.HexadecimalDigitExpected:
                    return "Hexadecimal digit expected";
                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }
        }
    }
}