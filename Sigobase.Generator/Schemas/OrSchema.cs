using System.Collections.Generic;
using System.Linq;
using Sigobase.Database;

namespace Sigobase.Generator.Schemas {
    class OrSchema : Schema {
        private HashSet<Schema> schemas = new HashSet<Schema>();

        public void Add(Schema schema) {
            schemas.Add(schema);
        }

        public override IEnumerable<ISigo> Values() {
            foreach (var schema in schemas) {
                foreach (var value in schema.Values()) {
                    yield return value;
                }
            }
        }

        public override int Count() => schemas.Select(x => x.Count()).Sum();

        public override string ToString() {
            return string.Join('|', schemas);
        }
    }
}