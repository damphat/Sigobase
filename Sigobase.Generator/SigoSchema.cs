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
            lock (Context) {
                Context[name] = value;
            }
        }

        public static SigoSchema GetType(string name) {
            lock (Context) {
                if (Context.TryGetValue(name, out var schema)) {
                    return schema;
                } else {
                    throw new InvalidOperationException($"schema '{name}' is not found");
                }
            }
        }

        #endregion

        public IEnumerable<ISigo> Generate() {
            return Generate(DefaultOptions);
        }

        public abstract IEnumerable<ISigo> Generate(GenerateOptions options);
        public abstract int Count();

        /// <summary>
        /// Execute and make a SigoSchema.
        /// 
        /// To make sure no other process change the context, lock it
        /// lock(SigoSchema.Context) { /*your code*/ }
        /// </summary>
        public static SigoSchema Parse(string schemaSource) {
            // lock the shared resource between threads
            lock (Context) {
                return new SchemaParser(schemaSource).Parse();
            }
        }
    }
}