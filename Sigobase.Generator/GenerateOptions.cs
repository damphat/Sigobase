using System;

namespace Sigobase.Generator {
    [Flags]
    public enum GenerateOptions {
        None = 0,
        Unique,
        Sorted,
        UniqueSorted = Unique | Sorted
    }
}