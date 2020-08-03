namespace Sigobase.Language.Utils {
    internal static class Chars {
        /// <summary>
        /// return an int 0..15, or -1
        /// </summary>
        public static int ToHex(in char c) {
            if (c >= '0' && c <= '9') {
                return c - '0';
            }

            if (c >= 'a' && c <= 'f') {
                return c - 'a' + 10;
            }

            if (c >= 'A' && c <= 'F') {
                return c - 'A' + 10;
            }

            return -1;
        }

        public static bool IsDigit(char c) {
            return c >= '0' && c <= '9';
        }

        public static bool IsIdentifierStart(char c) {
            return c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z' || c == '_';
        }

        public static bool IsIdentifierPart(char c) {
            return IsIdentifierStart(c) || IsDigit(c);
        }
    }
}