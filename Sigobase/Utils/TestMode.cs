using System;
using System.Collections.Generic;
using Sigobase.Database;

namespace Sigobase.Utils {
#if TESTMODE
    public static class TestMode {
        public static int Count { get; private set; } = 0;

        public static List<WeakReference<ISigo>> Caches {
            get {
                lock (_lock) {
                    int count = 0;
                    for (var i = 0; i < _caches.Count; i++) {
                        if (_caches[i] != null) {
                            if (_caches[i].TryGetTarget(out var target)) {
                                _caches[count++] = _caches[i];
                            } else {
                                _caches[i] = null;
                            }
                        }
                    }

                    _caches.RemoveRange(count, _caches.Count - count);

                    return _caches;
                }
            }
            set {
                lock (_lock) {
                    _caches = value;
                }
            }
        }

        private static readonly object _lock = new object();

        private static List<WeakReference<ISigo>> _caches;

        static TestMode() {
            _caches = new List<WeakReference<ISigo>>();
        }

        public static int GetID(ISigo t) {
            lock (_lock) {
                Caches.Add(new WeakReference<ISigo>(t));
                return Count++;
            }
        }
    }
#endif
}