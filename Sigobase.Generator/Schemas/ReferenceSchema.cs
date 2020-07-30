using System.Collections.Generic;
using Sigobase.Database;

namespace Sigobase.Generator.Schemas {
    internal class ReferenceSchema : Schema {
        public string Name { get; }

        public ReferenceSchema(string name) {
            Name = name;
        }

        public override IEnumerable<ISigo> Values(Options options) {
            return GetType(Name).Values(options);
        }

        public override int Count() {
            return GetType(Name).Count();
        }

        public override string ToString() {
            return Name;
        }
    }
}