﻿using System.Collections.Generic;
using Sigobase.Database;
using Sigobase.Utils;

namespace Sigobase.Implements {
    public static class ImplSet {
        public static ISigo SetN(ISigo sigo, IReadOnlyList<string> keys, ISigo value, int start) {
            switch (keys.Count - start) {
                case 0: return value;
                case 1: return sigo.Set1(keys[start], value);
                default: {
                    var key = keys[start];
                    return sigo.Set1(key, SetN(sigo.Get1(key), keys, value, start + 1));
                }
            }
        }

        public static ISigo Set(ISigo sigo, string path, ISigo value) {
            if (string.IsNullOrEmpty(path)) {
                return value;
            }

            if (!Paths.ShouldSplit(path)) {
                return sigo.Set1(path, value);
            }

            var keys = Paths.Split(path);
            return SetN(sigo, keys, value, 0);
        }
    }
}