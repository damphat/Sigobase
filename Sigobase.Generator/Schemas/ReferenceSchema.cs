using System.Collections.Generic;
using Sigobase.Database;

namespace Sigobase.Generator.Schemas {
    internal class ReferenceSchema : SigoSchema {
        public string Name { get; }

        public ReferenceSchema(string name) {
            Name = name;
        }

        public override IEnumerable<ISigo> Generate(GenerateOptions options) {
            return GetType(Name).Generate(options);
        }

        public override int Count() {
            return GetType(Name).Count();
        }

        public override string ToString() {
            return Name;
        }
    }
}