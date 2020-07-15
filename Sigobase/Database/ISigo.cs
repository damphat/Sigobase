using System;
using System.Collections.Generic;

namespace Sigobase.Database {
    public interface ISigo : IReadOnlyDictionary<string, ISigo>, IEquatable<ISigo> {
        int Flags { get; }
        object Data { get; }

        ISigo Get1(string key);
        ISigo Set1(string key, ISigo value);

        ISigo Freeze();
    }
}