using System;
using System.Collections.Generic;
using Sigobase.Utils;

namespace Sigobase.Database {
    public interface ISigo : IReadOnlyDictionary<string, ISigo>, IEquatable<ISigo> {
        int Flags { get; }
        object Data { get; }

        ISigo Get1(string key);
        ISigo Set1(string key, ISigo value);

        ISigo Freeze();

#if TESTMODE
        TestInfo Info { get; }
#endif
    }
}