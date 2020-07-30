using System.Collections.Generic;
using Sigobase.Database;

namespace Sigobase.Generator.Utils {
    internal class SigoHashSet : HashSet<ISigo> {
        public SigoHashSet(IEnumerable<ISigo> collection) : base(collection) { }
        public SigoHashSet() { }
    }
}