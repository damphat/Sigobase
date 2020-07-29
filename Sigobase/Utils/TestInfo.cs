using System;
using System.Collections.Generic;
using System.Text;
using Sigobase.Database;

namespace Sigobase.Utils {
#if TESTMODE
    public class TestInfo {
        private static List<TestInfo> caches;

        static TestInfo() {
            caches = new List<TestInfo>();
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
                lock (caches) {
                    var count = 0;
                    for (var i = 0; i < caches.Count; i++) {
                        if (caches[i] != null) {
                            if (caches[i].Target.TryGetTarget(out var target)) {
                                caches[count++] = caches[i];
                            } else {
                                caches[i] = null;
                            }
                        }
                    }

                    caches.RemoveRange(count, caches.Count - count);

                    return caches;
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
            lock (caches) {
                var info = new TestInfo(t);
                caches.Add(info);
                return info;
            }
        }
    }
#endif
}