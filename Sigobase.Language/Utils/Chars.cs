using System.Runtime.CompilerServices;

namespace Sigobase.Language.Utils {
    internal static class Chars {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDigit(char c) {
            return c >= '0' && c <= '9';
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsIdentifierStart(char c) {
            return c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z' || c == '_';
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsIdentifierPart(char c) {
            return IsIdentifierStart(c) || IsDigit(c);
        }
    }
}