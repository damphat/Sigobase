using System.Collections.Generic;
using Sigobase.Database;
using Sigobase.Utils;
using Xunit;

namespace Sigobase.Tests {
    public class Set1_leaf_tests {
        private ISigo s = Sigo.From("s");

        [Fact]
        public void Return_self() {
            Assert.Same(s, s.Set1("k", Sigo.Create(3)));
        }

        [Fact]
        public void Return_element7() {
            Assert.Same(Sigo.Create(7), s.Set1("k", Sigo.Create(7)));
        }

        [Fact]
        public void Return_treeWith1Child() {
            var values = new List<ISigo> {
                Sigo.From("v"),
                Sigo.Create(0),
                Sigo.Create(1),
                Sigo.Create(2),
                Sigo.Create(3, "k", Sigo.Create(0)),
                Sigo.Create(4),
                Sigo.Create(5),
                Sigo.Create(6),
                Sigo.Create(7, "k", "v")
            };

            foreach (var v in values) {
                var ret = s.Set1("k", v);
                Assert.Same(v, ret.Get1("k"));
            }
        }
    }

    public class Set1_tree_tests {
        private ISigo v1 = Sigo.From("v1");
        private ISigo v2 = Sigo.From("v2");
        private ISigo e0 = Sigo.Create(0);
        private ISigo e3 = Sigo.Create(3);
        private ISigo e4 = Sigo.Create(4);
        private ISigo e6 = Sigo.Create(6);
        private ISigo e7 = Sigo.Create(7);

        [Fact]
        public void AddNewChild() {
            var s = Sigo.Create(0);
            s = s.Set1("k1", v1);
            s = s.Set1("k2", v2);

            Assert.Equal(v1, s.Get1("k1"));
            Assert.Equal(v2, s.Get1("k2"));
            Assert.Equal(256 * 2 + 6, s.Flags);
        }

        [Fact]
        public void RemoveAnExistingChild() {
            var s = e0;
            // add
            s = s.Set1("k1", v1);
            s = s.Set1("k2", v2);

            // remove k1
            s = s.Set1("k1", e0);
            Assert.Equal(e0, s.Get1("k1"));
            Assert.Equal(v2, s.Get1("k2"));
            Assert.Equal(256 + 6, s.Flags);

            // remove k2
            s = s.Set1("k2", e4);
            Assert.Equal(e0, s.Get1("k1"));
            Assert.Equal(e0, s.Get1("k2"));
            Assert.Equal(e6, s);
        }

        [Fact]
        public void ChangeAnExistingChild() {
            var s = e0;
            // add
            s = s.Set1("k", v1);

            // set again
            s = s.Set1("k", v2);

            Assert.Equal(v2, s.Get1("k"));
            Assert.Equal(256 + 6, s.Flags);
        }

        [Fact]
        public void ChangeFlagsOnly() {
            var s0 = e0.Set1("k", e3);

            // change parent proton by adding a child with L effect
            var s1 = s0.Set1("k2", e4);

            Assert.Same(s0, s1);
            Assert.Equal(e3, s1.Get1("k"));
            Assert.Equal(Bits.LM, s1.Flags & Bits.LMR);
        }

        [Fact]
        public void Donot_createNewObject_ifNothingChange() {
            var v = Sigo.From("v");
            var s = e3.Set1("k", v).Freeze();

            Assert.Same(s, s.Set1("k", v));
            Assert.Same(s, s.Set1("k1", e3));
            Assert.Same(s, s.Set1("k1", e7));

            // using Sigo.Same(), insteadOf referenceEquals
            Assert.Same(s, s.Set1("k", Sigo.From("v")));
        }
    }

    public class Set1_tree_frozen_tests {
        private ISigo v1 = Sigo.From("v1");
        private ISigo v2 = Sigo.From("v2");
        private ISigo e0 = Sigo.Create(0);
        private ISigo e3 = Sigo.Create(3);
        private ISigo e4 = Sigo.Create(4);
        private ISigo e6 = Sigo.Create(6);

        [Fact]
        public void AddNewChild() {
            var s0 = Sigo.Create(0);
            var s1 = s0.Set1("k1", v1).Freeze();
            var s2 = s1.Set1("k2", v2).Freeze();

            Assert.Equal(v1, s2.Get1("k1"));
            Assert.Equal(v2, s2.Get1("k2"));
            Assert.Equal(256 * 2 + 16 + 6, s2.Flags);
        }

        [Fact]
        public void RemoveAnExistingChild() {
            var s0 = Sigo.Create(0);
            // add
            var s1 = s0.Set1("k1", v1).Freeze();
            var s2 = s1.Set1("k2", v2).Freeze();

            // remove k1
            var s3 = s2.Set1("k1", e0).Freeze();
            Assert.Equal(e0, s3.Get1("k1"));
            Assert.Equal(v2, s3.Get1("k2"));
            Assert.Equal(256 + 16 + 6, s3.Flags);

            // remove k2
            var s4 = s3.Set1("k2", e4).Freeze();
            Assert.Equal(e0, s4.Get1("k1"));
            Assert.Equal(e0, s4.Get1("k2"));
            Assert.Equal(e6, s4);
        }

        [Fact]
        public void ChangeAnExistingChild() {
            var s0 = Sigo.Create(0);
            // add
            var s1 = s0.Set1("k", v1).Freeze();

            // set again
            var s2 = s1.Set1("k", v2).Freeze();

            Assert.Equal(v2, s2.Get1("k"));
            Assert.Equal(256 + 16 + 6, s2.Flags);
        }

        [Fact]
        public void ChangeFlagsOnly() {
            var s0 = e0.Set1("k", e3).Freeze();

            // change parent proton by adding a child with L effect
            var s1 = s0.Set1("k2", e4).Freeze();

            Assert.Equal(e3, s1.Get1("k"));
            Assert.Equal(Bits.LM, s1.Flags & Bits.LMR);
        }
    }
}