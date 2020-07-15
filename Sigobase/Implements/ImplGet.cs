using System;
using System.Linq;
using Sigobase.Database;

namespace Sigobase.Implements {
    public static class ImplGet {
        /// <summary>
        /// A looped sigo is a sigo that returns itself
        /// The result is depends on how the default children is defined
        /// </summary>
        public static bool IsLooped(ISigo sigo) {
            switch (sigo.Flags & (-256 + 8 + 7)) {
                case 0:
                case 3:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// <code>
        /// sigo.Get("user/name") => sigo.Get1("user").Get1("name")
        /// sigo.Get("user") => sigo.Get1("user")
        /// sigo.Get("/") => sigo
        /// </code>
        /// </summary>
        public static ISigo Get(ISigo sigo, string path) {
            if (IsLooped(sigo)) {
                return sigo;
            }

            if (string.IsNullOrEmpty(path)) {
                return sigo;
            }

            if (!path.Contains('/')) {
                return sigo.Get1(path);
            }

            var keys = path.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries);

            foreach (var key in keys) {
                sigo = sigo.Get1(key);
                if (IsLooped(sigo)) {
                    return sigo;
                }
            }

            return sigo;
        }
    }
}