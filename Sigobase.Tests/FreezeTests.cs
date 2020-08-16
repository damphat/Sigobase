using Sigobase.Database;
using Xunit;

namespace Sigobase.Tests {
    public class FreezeTests {
        private ISigo tree = Sigo.Create(3, "k", "v");
        private ISigo leaf = Sigo.From("v");
        private ISigo e3 = Sigo.Create(3);

        [Fact]
        public void Freeze_always_returns_self() {
            SigoAssert.Same(tree, tree.Freeze());
            SigoAssert.Same(leaf, leaf.Freeze());
            SigoAssert.Same(e3, e3.Freeze());
        }

        [Fact]
        public void NonEmptyTree_areNotFrozen_after_creating_or_changing() {
            // non empty tree are not frozen after creating
            SigoAssert.False(tree.IsFrozen());

            // not frozen after changing
            tree = tree.Freeze().Set1("v", Sigo.From("v+"));
            SigoAssert.False(tree.IsFrozen());

            // leafs and elements are frozen always frozen
            SigoAssert.True(leaf.IsFrozen());
            SigoAssert.True(e3.IsFrozen());
        }

        [Fact]
        public void Frozen_a_nonFrozen() {
            tree.Freeze();
            SigoAssert.True(tree.IsFrozen());
        }
    }
}