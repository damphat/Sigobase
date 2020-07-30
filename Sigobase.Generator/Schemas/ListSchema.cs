using System.Collections.Generic;
using System.Linq;
using Sigobase.Database;
using Sigobase.Generator.Utils;

namespace Sigobase.Generator.Schemas {
    internal class ListSchema : Schema {
        public IReadOnlyList<Schema> Items { get; }

        public ListSchema(IReadOnlyList<Schema> items) {
            Items = items;
        }

        private IEnumerable<ISigo> Generate(Options filter) {
            return Items.SelectMany(schema => schema.Values(filter));
        }

        public override IEnumerable<ISigo> Values(Options options) {
            switch (options) {
                case Options.Unique: {
                    var ret = new SigoHashSet(Generate(options));
                    return ret;
                }
                case Options.Sorted: {
                    var ret = new List<ISigo>(Generate(options));
                    ret.Sort(new SigoComparer());
                    return ret;
                }
                case Options.Unique | Options.Sorted: {
                    var ret = new SigoSortedSet(Generate(options));
                    return ret.ToList();
                }
                case Options.None:
                default: return Generate(options);
            }
        }

        public override int Count() {
            return Items.Select(x => x.Count()).Sum();
        }

        public override string ToString() {
            return string.Join("|", Items);
        }
    }
}