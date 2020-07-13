using System.Collections.Generic;

namespace Sigobase {
    public interface ISigo {
        int Flags { get; }
        object Data { get; }

        ISigo Get1(string key);
        ISigo Set1(string key, ISigo value);
        IEnumerable<string> Keys { get; }

        ISigo Freeze();
    }
}