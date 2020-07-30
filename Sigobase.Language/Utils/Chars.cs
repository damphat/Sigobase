namespace Sigobase.Language.Utils {
    internal static class Chars {
        public static int ToHex(in char c) {
            if (c >= '0' && c <= '9') return c - '0';
            if (c >= 'a' && c <= 'f') return c - 'a' + 10;
            if (c >= 'A' && c <= 'F') return c - 'A' + 10;
            return -1;
        }

        public static bool Digit(char c) {
            return c >= '0' && c <= '9';
        }

        public static bool IdentifierStart(char c) {
            return c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z' || c == '_';
        }

        public static bool IdentifierPart(char c) {
            return c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z' || c == '_' || c >= '0' && c <= '9';
        }


    }
}