using System.Collections.Generic;
using Sigobase.Database;

namespace Sigobase.Generator.Schemas {
    class SingleValueSchema : Schema {
        public ISigo Value;

        public SingleValueSchema(object value) {
            Value = Sigo.From(value);
        }

        public override IEnumerable<ISigo> Values() {
            yield return Value;
        }

        public override int Count() {
            return 1;
        }

        public override string ToString() {
            return Value.ToString();
        }
    }
}