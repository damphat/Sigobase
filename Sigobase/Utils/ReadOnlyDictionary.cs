using System.Collections;
using System.Collections.Generic;
using Sigobase.Database;

namespace Sigobase.Utils {
    /// <summary>
    /// An implement of IReadOnlyDictionary
    /// Allow mutable operation: add, set, remove
    /// Allow immutable operation: clone, add, set, remove
    /// </summary>
    internal class ReadOnlyDictionary : IReadOnlyDictionary<string, ISigo> {
        private readonly Dict dict;

        public ReadOnlyDictionary(Dict dict) {
            this.dict = dict;
        }

        public IEnumerator<KeyValuePair<string, ISigo>> GetEnumerator() {
            return dict.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return ((IEnumerable) dict).GetEnumerator();
        }

        public int Count => dict.Count;

        public bool ContainsKey(string key) {
            return dict.ContainsKey(key);
        }

        public bool TryGetValue(string key, out ISigo value) {
            return dict.TryGetValue(key, out value);
        }

        public ISigo this[string key] => dict[key];

        public IEnumerable<string> Keys => dict.Keys;

        public IEnumerable<ISigo> Values => dict.Values;

        public Dict DictAdd(string key, ISigo value) {
            dict.Add(key, value);
            return dict;
        }

        public Dict DictClone() {
            return new Dict(dict);
        }

        public Dict DictCloneAdd(string key, ISigo value) {
            return new Dict(dict) {{key, value}};
        }

        public Dict DictSet(string key, ISigo value) {
            dict[key] = value;
            return dict;
        }

        public Dict DictCloneSet(string key, ISigo value) {
            return new Dict(dict) {
                [key] = value
            };
        }

        public Dict DictRemove(string key) {
            dict.Remove(key);
            return dict;
        }

        public Dict DictCloneRemove(string key) {
            var ret = new Dict(dict);
            ret.Remove(key);
            return ret;
        }
    }
}