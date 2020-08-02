using System;
using System.Collections.Generic;
using Sigobase.Database;
using Sigobase.Generator.Utils;

namespace Sigobase.Generator {

    // bad design: this class also contains a global context
    public abstract class SigoSchema {
        #region static

        public const GenerateOptions DefaultOptions = GenerateOptions.UniqueSorted;
        public static readonly Dictionary<string, SigoSchema> Context = new Dictionary<string, SigoSchema>();

        public static void SetType(string name, SigoSchema value) {
            Context[name] = value;
        }

        public static SigoSchema GetType(string name) {
            if (Context.TryGetValue(name, out var value)) {
                return value;
            } else {
                if (Context.TryGetValue(name, out var schema)) {
                    return schema;
                } else {
                    throw new Exception($"schema '{name}' is not found");
                }
            }
        }

        #endregion

        public IEnumerable<ISigo> Generate() {
            return Generate(DefaultOptions);
        }

        public abstract IEnumerable<ISigo> Generate(GenerateOptions options);
        public abstract int Count();

        // TODO actually eval()
        public static SigoSchema Parse(string schemaSource) {
            return new SchemaParser(schemaSource).Parse();
        }
    }
}