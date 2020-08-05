using System.Collections.Generic;
using Sigobase.Database;

namespace Sigobase.Generator.Schemas {
    // TODO this should be singleton
    internal class NothingSchema : SigoSchema {
        public override IEnumerable<ISigo> Generate(GenerateOptions options) {
            yield break;
        }

        public override int Count() {
            return 0;
        }

        public override string ToString() {
            return "";
        }
    }
}