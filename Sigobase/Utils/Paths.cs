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
    }
}