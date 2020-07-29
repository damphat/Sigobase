using System.Collections.Generic;
using Sigobase.Database;
using Sigobase.Generator.Utils;

namespace Sigobase.Generator.Schemas {
    abstract class Schema {
        private SigoHashSet caches;

        public IEnumerable<ISigo> Values(bool filter) {
            if (filter == false) {
                foreach (var value in Values()) {
                    yield return value;
                }
            } else {
                if (caches == null) {
                    caches = new SigoHashSet();
                } else {
                    caches.Clear();
                }

                foreach (var value in Values()) {
                    if (caches.Add(value)) {
                        yield return value;
                    }
                }
            }
        }

        public abstract IEnumerable<ISigo> Values();

        public abstract int Count();
    }
}