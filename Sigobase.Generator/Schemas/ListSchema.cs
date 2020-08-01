using System.Collections.Generic;
using System.Linq;
using Sigobase.Database;
using Sigobase.Generator.Utils;

namespace Sigobase.Generator.Schemas {
    internal class ListSchema : SigoSchema {
        public IReadOnlyList<SigoSchema> Items { get; }

        public ListSchema(IReadOnlyList<SigoSchema> items) {
            Items = items;
        }

        private IEnumerable<ISigo> GenerateInternal(GenerateOptions filter) {
            return Items.SelectMany(schema => schema.Generate(filter));
        }

        public override IEnumerable<ISigo> Generate(GenerateOptions options) {
            switch (options) {
                case GenerateOptions.Unique: {
                    var ret = new SigoHashSet(GenerateInternal(options));
                    return ret;
                }
                case GenerateOptions.Sorted: {
                    var ret = new List<ISigo>(GenerateInternal(options));
                    ret.Sort(new SigoComparer());
                    return ret;
                }
                case GenerateOptions.Unique | GenerateOptions.Sorted: {
                    var ret = new SigoSortedSet(GenerateInternal(options));
                    return ret.ToList();
                }
                case GenerateOptions.None:
                default: return GenerateInternal(options);
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