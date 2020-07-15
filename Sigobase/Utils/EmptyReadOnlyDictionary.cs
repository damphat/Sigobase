using System.Collections;
using System.Collections.Generic;
using Sigobase.Database;

namespace Sigobase.Utils {
    internal class EmptyReadOnlyDictionary : IReadOnlyDictionary<string, ISigo> {
        public IEnumerator<KeyValuePair<string, ISigo>> GetEnumerator() {
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public int Count => 0;

        public bool ContainsKey(string key) {
            throw new System.NotImplementedException();
        }

        public bool TryGetValue(string key, out ISigo value) {
            value = default;
            return false;
        }

        public ISigo this[string key] => throw new System.NotImplementedException();

        public IEnumerable<string> Keys {
            get { yield break; }
        }

        public IEnumerable<ISigo> Values {
            get { yield break; }
        }
    }
}