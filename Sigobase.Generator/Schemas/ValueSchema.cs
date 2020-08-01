using System.Collections.Generic;
using Sigobase.Database;

namespace Sigobase.Generator.Schemas {
    internal class ValueSchema : SigoSchema {
        public ISigo Value { get; }

        public ValueSchema(object value) {
            Value = Sigo.From(value);
        }

        public override IEnumerable<ISigo> Generate(GenerateOptions options) {
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