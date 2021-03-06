﻿using System;
using System.Collections;
using System.Collections.Generic;
using Sigobase.Database;
using Xunit;

namespace Sigobase.Tests {
    public class FromTests {
        [Fact]
        public void Return_leaf_if_input_is_scalar() {
            var inputs = new object[] {
                true, false,
                123.0,
                "",
                "abc"
            };

            foreach (var scalar in inputs) {
                var sigo = Sigo.From(scalar);

                SigoAssert.True(sigo.IsLeaf());
                SigoAssert.Equal(scalar, sigo.Data);
            }
        }

        [Fact]
        public void Numbers_are_convert_to_double() {
            var inputs = new object[] {
                (int) 1,
                (long) 1,
                (float) 1,
                (double) 1
            };

            foreach (var number in inputs) {
                var sigo = Sigo.From(number);

                SigoAssert.Equal(Convert.ToDouble(number), sigo.Data);
            }
        }

        [Fact]
        public void Return_the_input_if_input_is_sigo() {
            var inputs = new object[] {
                Sigo.From("abc"),
                Sigo.Create(3),
                Sigo.Create(3).Set1("k", Sigo.From("v"))
            };

            foreach (var o in inputs) {
                var sigo = Sigo.From(o);

                SigoAssert.Same(o, sigo);
            }
        }

        [Fact]
        public void Convert_to_tree_if_input_is_dictionary() {
            var inputs = new object[] {
                new Hashtable() {{"k", "v"}},
                new Dictionary<string, object>() {{"k", "v"}}
            };

            foreach (var o in inputs) {
                var sigo = Sigo.From(o);

                SigoAssert.True(sigo.IsTree());
                SigoAssert.Equal("v", sigo.Get1("k").Data);
            }
        }

        [Fact]
        public void Convert_to_tree_if_input_is_enumerable() {
            var inputs = new object[] {
                new[] {"a", "b"},
                new List<string> {"a", "b"}
            };

            foreach (var o in inputs) {
                var sigo = Sigo.From(o);

                SigoAssert.True(sigo.IsTree());
                SigoAssert.Equal("a", sigo.Get1("0").Data);
                SigoAssert.Equal("b", sigo.Get1("1").Data);
            }
        }
    }
}