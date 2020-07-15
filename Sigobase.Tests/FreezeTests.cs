using Sigobase.Database;
using Xunit;

namespace Sigobase.Tests {
    public class FreezeTests {
        [Fact]
        public void Trees() {
            var user0 = Sigo.Create(3, "name", "Phat");

            // trees are mutable before calling freeze()
            Assert.False(user0.IsFrozen());

            // freeze twice
            var user1 = user0.Freeze();
            var user2 = user1.Freeze();

            Assert.True(user2.IsFrozen());
            Assert.Same(user0, user1);
            Assert.Same(user1, user2);
        }

        [Fact]
        public void Leafs() {
            var v0 = Sigo.From("v");

            // leafs are frozen by default
            Assert.True(v0.IsFrozen());

            // return self
            var v1 = v0.Freeze();
            Assert.Same(v0, v1);
            Assert.True(v1.IsFrozen());
        }
    }
}