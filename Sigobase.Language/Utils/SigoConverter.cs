using System.Globalization;
using System.Runtime.CompilerServices;

namespace Sigobase.Language.Utils {
    internal static class SigoConverter {
        /// <summary>
        /// Convert a hex-digit to int 0..15
        /// Return -1 if input is not a hex-digit
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Hex2Int(char c) {
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

        // FIXME ToDouble("1e1000") may throw OverflowException, or return Infinity
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ToDouble(string str) {
            return double.Parse(str, CultureInfo.InvariantCulture);
        }
    }
}