using System.Collections.Generic;
using Sigobase.Database;

namespace Sigobase.Generator.Utils {
    internal class SigoHashSet : HashSet<ISigo> {
        private static readonly SigoEqualityComparer comparer = new SigoEqualityComparer();
        public SigoHashSet(IEnumerable<ISigo> collection) : base(collection, comparer) { }
        public SigoHashSet() : base(comparer) { }
    }

    internal class SigoSortedSet : SortedSet<ISigo> {
        private static readonly SigoComparer comparer = new SigoComparer();
        public SigoSortedSet(IEnumerable<ISigo> collection) : base(collection, comparer) { }
        public SigoSortedSet() : base(comparer) { }
    }
}