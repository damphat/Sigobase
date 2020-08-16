using Sigobase.Database;
using Xunit;

namespace Sigobase.Tests {
    public class CloneTests {
        [Fact]
        public void Frozen_return_self() {
            var leaf = Sigo.From("leaf");
            var e3 = Sigo.Create(3);
            var tree = Sigo.Create(3, "k", "v").Freeze();

            SigoAssert.Same(leaf, leaf.Clone());
            SigoAssert.Same(e3, e3.Clone());
            SigoAssert.Same(tree, tree.Clone());
        }

        [Fact]
        public void NonFrozen_return_new_obj() {
            var user = Sigo.Create(3,
                "name", Sigo.Create(3,
                    "first", "Phat",
                    "last", "Dam"),
                "frozen", Sigo.Create(0, "k", "v").Freeze()
            );

            SigoAssert.False(user.IsFrozen());
            SigoAssert.False(user.Get("name").IsFrozen());
            SigoAssert.True(user.Get("frozen").IsFrozen());

            var clone = user.Clone();

            SigoAssert.NotSame(user, clone);
            SigoAssert.NotSame(user.Get("name"), clone.Get("name"));
            SigoAssert.Same(user.Get("frozen"), clone.Get("frozen"));
        }
    }
}