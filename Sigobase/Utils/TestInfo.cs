using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Sigobase.Database;

namespace Sigobase.Utils {
#if TESTMODE
    public class TestInfo {

        private static List<TestInfo> _caches;

        static TestInfo() {
            _caches = new List<TestInfo>();
        }

        private TestInfo(ISigo target) {
            Id = Count++;
            Target = new WeakReference<ISigo>(target);
        }

        public int Id { get; }
        public WeakReference<ISigo> Target { get; }

        public static int Count { get; private set; }

        public static List<TestInfo> Caches {
            get {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                lock (_caches) {
                    var count = 0;
                    for (var i = 0; i < _caches.Count; i++) {
                        if (_caches[i] != null) {
                            if (_caches[i].Target.TryGetTarget(out var target)) {
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
        }

        public override string ToString() {
            if (!Target.TryGetTarget(out var sigo)) {
                return $"dead({Id})";
            } else {
                if (sigo.IsLeaf()) return sigo.ToString();
                var sb = new StringBuilder();
                sb.Append('{');
                if (sigo.IsFrozen()) sb.Append('*');
                sb.Append(Id);
                sb.Append('+');
                sb.Append(Bits.Proton(sigo.Flags));
                foreach (var e in sigo) {
                    sb.Append(',');
                    sb.Append(e.Key).Append(':').Append(e.Value.Info.ToString());
                }

                sb.Append('}');
                return sb.ToString();
            }
        }

        public static TestInfo CreateInfo(ISigo t) {
            lock (_caches) {
                var info = new TestInfo(t);
                Caches.Add(info);
                return info;
            }
        }
    }
#endif
}