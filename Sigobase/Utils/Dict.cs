using System.Collections.Generic;
using Sigobase.Database;

namespace Sigobase.Utils {
    /// <summary>
    /// This is used as both mutable and immutable scenarios.
    /// Create or find a better dict for immutable data
    /// </summary>
    internal class Dict : Dictionary<string, ISigo> {
        public Dict() { }
        public Dict(Dict dict) : base(dict) { }
    }
}