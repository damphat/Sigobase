using System;
using System.Linq;

namespace Sigobase.Utils {
    public static class Paths {
        public static void CheckKey(string key) {
            if (string.IsNullOrEmpty(key)) {
                throw new ArgumentException($"'{key}' is not a valid key");
            }

            if (key.Contains('/')) {
                throw new ArgumentException($"'{key}' contains '/'");
            }
        }

        public static bool ShouldSplit(string path) {
            return path.Contains('/');
        }

        public static string[] Split(string path) {
            return path.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries);
        }

        public static bool IsIdentifierOrInteger(string key) {
            if (string.IsNullOrEmpty(key)) {
                return false;
            }

            var c = key[0];

            if (c == '0') {
                return key.Length == 1;
            } else if (c >= '1' && c <= '9') {
                for (var i = 1; i < key.Length; i++) {
                    c = key[i];
                    if (c < '0' || c > '9') {
                        return false;
                    }
                }

                return true;
            } else if (c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z' || c == '_') {
                for (var i = 1; i < key.Length; i++) {
                    c = key[i];
                    if ((c < 'a' || c > 'z') && (c < 'A' || c > 'Z') && c != '_' && (c < '0' || c > '9')) {
                        return false;
                    }
                }

                return true;
            } else {
                return false;
            }
        }
    }
}