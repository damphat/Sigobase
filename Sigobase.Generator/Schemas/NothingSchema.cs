using System.Collections.Generic;
using Sigobase.Database;

namespace Sigobase.Generator.Schemas {
    internal class NothingSchema : Schema {
        public override IEnumerable<ISigo> Values(Options options) {
            yield break;
        }

        public override int Count() {
            return 0;
        }
    }
}