using System;
using System.Collections.Generic;
using Sigobase.Database;

namespace Sigobase.Generator.Schemas {
    internal abstract class Schema {
        #region static

        private const Options DefaultOptions = Options.UniqueSorted;
        public static readonly Dictionary<string, Schema> SchemaDict = new Dictionary<string, Schema>();

        public static void SetType(string name, Schema value) {
            SchemaDict[name] = value;
        }

        public static Schema GetType(string name) {
            if (SchemaDict.TryGetValue(name, out var value)) {
                return value;
            } else {
                if (SchemaDict.TryGetValue(name, out var schema)) {
                    return schema;
                } else {
                    throw new Exception($"schema '{name}' is not found");
                }
            }
        }

        #endregion

        public IEnumerable<ISigo> Values() {
            return Values(DefaultOptions);
        }

        public abstract IEnumerable<ISigo> Values(Options options);
        public abstract int Count();
    }
}