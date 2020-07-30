using System;

namespace Sigobase.Generator.Schemas {
    [Flags]
    public enum Options {
        None = 0,
        Unique,
        Sorted,
        UniqueSorted = Unique | Sorted
    }
}