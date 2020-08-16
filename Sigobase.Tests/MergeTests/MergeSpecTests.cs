using Sigobase.Database;
using Sigobase.Implements;
using Xunit;

namespace Sigobase.Tests.MergeTests {
    public class MergeSpecTests {
        [Fact(Skip = "Not Implemented")]
        public void Try_to_return_part_of_a() {
            var a = Sigo.Create(7,
                "x/name", "Phat"
            ).Freeze();

            var b = Sigo.Create(7,
                "x/name", "Phat",
                "y", "y"
            ).Freeze();

            var r = ImplMergeSpec.Merge(a, b);

            // r equal b...
            SigoAssert.Equal(r, b);

            // ...but not same b...
            SigoAssert.NotSame(r, b);

            // ...because there is r[x] == a[x]
            SigoAssert.Equal(r["x"], a["x"]);
            SigoAssert.Same(r["x"], a["x"]);
        }
    }
}