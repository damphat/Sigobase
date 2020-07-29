using System.Collections.Generic;
using Sigobase.Database;

namespace Sigobase.Generator.Utils {
    internal class SigoHashSet : HashSet<ISigo> {
        public SigoHashSet() : base(new SigoEqualityComparer()) { }
    }
}