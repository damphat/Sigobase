using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Sigobase.Database;
using Sigobase.Generator;
using Xunit;

namespace Sigobase.Language.Tests {
    public class ParserTests {
        [Theory]
        [InlineData(" 1")]
        [InlineData("+1")]
        [InlineData("-1")]
        [InlineData(" Infinity")]
        [InlineData("+Infinity")]
        [InlineData("-Infinity")]
        [InlineData(" NaN")]
        [InlineData("+NaN")]
        [InlineData("-NaN")]
        public void Leaf_numberTests(string src) {
            var num = double.Parse(src, CultureInfo.InvariantCulture);

            Assert.Equal(Sigo.From(num), Sigo.Parse(src));
        }

        [Theory]
        [InlineData("true", true)]
        [InlineData("false", false)]
        public void Leaf_boolTests(string src, bool value) {
            Assert.Equal(Sigo.From(value), Sigo.Parse(src));
        }

        [Theory]
        [InlineData("'abc'", "abc")]
        [InlineData("\"abc\"", "abc")]
        [InlineData("'\\\\'", "\\")]
        [InlineData(@"'\b\f\r\n\t'", "\b\f\r\n\t")]
        [InlineData(@"'\u0123\u4567\u8901'", "\u0123\u4567\u8901")]
        [InlineData(@"'\uabcd\uefAB\uCDEF'", "\uabcd\uefAB\uCDEF")]
        public void Leaf_stringTests(string src, string value) {
            Assert.Equal(Sigo.From(value), Sigo.Parse(src));
        }

        [Theory]
        [InlineData(" ' \" ' ", " \" ")]
        [InlineData(" \" \\\" \" ", " \" ")]
        [InlineData(" ' \\' '", " ' ")]
        [InlineData(" \" ' \"", " ' ")]
        public void Leaf_stringQuoteTests(string src, string value) {
            Assert.Equal(Sigo.From(value), Sigo.Parse(src));
        }

        [Theory]
        [MemberData(nameof(ObjectData), null)]
        public void Object_tests(SigoWraper wraper) {
            Assert.Equal(wraper.Sigo, Sigo.Parse(wraper.ToString()));
        }

        [Theory]
        [MemberData(nameof(ObjectData), "{?}")]
        public void Object_emptyTests(SigoWraper wraper) {
            Assert.Equal(wraper.Sigo, Sigo.Parse(wraper.ToString()));
        }

        [Theory ()]
        [InlineData("true")]
        [InlineData("false")]
        [InlineData("Infinity")]
        [InlineData("NaN")]
        [InlineData("123")]
        [InlineData("'abc'")]
        public void TODO_Assign_to_literal(string variableName) {
            Assert.ThrowsAny<Exception>(() => Sigo.Parse(variableName + " = 1"));
        }

        [Theory]
        [InlineData("{1,}")]
        [InlineData("{1;}")]
        [InlineData("{1, x:1}")]
        [InlineData("{1, x:1,}")]
        [InlineData("{1; x:1;}")]
        [InlineData("{1, x:1, y:1}")]
        [InlineData("{1; x:1; y:1}")]
        public void Object_separatorTests(string src) {
            string Clean(string src) {
                src = src
                    .Replace(' ', ',')
                    .Replace(';', ',')
                    .Replace("}", ",}");
                src = src.Replace(",,", ",").Replace(",,", ",");
                return src;
            }

            Assert.Equal(Sigo.Parse(Clean(src)), Sigo.Parse(src));
        }

        public static IEnumerable<object[]> ObjectData(string schema = null) {
            schema ??= @"
                name = {03, first:'a', last?:'b'};
                user = {03, name?, age?: 1};
                // return
                user";
            return SigoSchema.Parse(schema).Generate().Select(s => new[] {new SigoWraper(s)});
        }

        [Fact]
        public void Object_pathTest() {
            var expected = Sigo.Create(3, "name", Sigo.Create(3, "first", 1, "last", 2));
            Assert.Equal(expected, Sigo.Parse("{name/first:1, name/last:2}"));
        }

        [Theory]
        [InlineData("[]", "{}")]
        [InlineData("[ ]", "{}")]
        [InlineData("[0]",  "{0:0}")]
        [InlineData("[0 ]", "{0:0}")]
        [InlineData("[0,]", "{0:0}")]
        [InlineData("[0;]", "{0:0}")]
        [InlineData("[0 1 ]", "{0:0,1:1}")]
        [InlineData("[0,1,]", "{0:0,1:1}")]
        [InlineData("[0;1;]", "{0:0,1:1}")]
        [InlineData("[[8,9]]", "{0:{0:8,1:9}}")]
        public void TODO_ArrayTest(string src, string expect) {
            Assert.Equal(Sigo.Parse(expect), Sigo.Parse(src));
        }

        [Theory]
        [InlineData("{true:1}", "{'true':1}")]
        [InlineData("{false:1}", "{'false':1}")]
        [InlineData("{Infinity:1}", "{'Infinity':1}")]
        [InlineData("{NaN:1}", "{'NaN':1}")]
        [InlineData("{1.5:1}", "{'1.5':1}")]
        public void TODO_PathTest(string src, string expect) {
            Assert.Equal(Sigo.Parse(expect), Sigo.Parse(src));
        }

        [Theory]
        [InlineData("{,}")]
        [InlineData("{,1}")]
        [InlineData("{1x:1}")]
        [InlineData("{x:1y:1}")]
        public void Object_errorTests(string src) {
            Assert.ThrowsAny<Exception>(() => Sigo.Parse(src));
        }

        [Theory]
        //unexpected separator
        [InlineData("[,]")]
        [InlineData("[,0]")]
        [InlineData("[0,,true]")]
        // ',' expected
        [InlineData("[0true]")]
        // ']' expected
        [InlineData(" [")]
        [InlineData(" [0")]
        // value expected
        [InlineData(" [0,")]
        public void TODO_ArrayErrorTest(string src) {
            Assert.ThrowsAny<Exception>(() => Sigo.Parse(src));
        }

        [Theory()]
        [InlineData("x=1; {x}", "{x:1}")]
        [InlineData("x=1; {a/x}", "{a/x:1}")]
        public void TODO_ContextTest(string a, string b) {
            Assert.Equal(Sigo.Parse(a), Sigo.Parse(b));
        }

        [Fact(Skip = "TODO")]
        public void TODO_MergeTest() {
            Sigo.Parse("{name: 'Phat'} | {0, age: 10}");
        }

        [Fact (Skip = "TODO")]
        public void TODO_SelectTest() {
            Sigo.Parse("select{name:{first, last}, friends:{0, 2, 4}}");
        }

        [Fact(Skip = "TODO")]
        public void TODO_ExpressionTest() {
            Sigo.Parse("(1+2)*3");
        }

        [Theory()]
        [InlineData("{'':1} ", "1")]
        [InlineData("{'/':1} ", "1")]
        [InlineData("{'a':1} ", "{a:1}")]
        [InlineData("{'/a':1}", "{a:1}")]
        [InlineData("{'a/':1}", "{a:1}")]
        [InlineData("{'a/b':1}", "{a:{b:1}}")]
        [InlineData("{'/a//b/':1}", "{a:{b:1}}")]
        [InlineData("{'a'/'b':1}", "{a:{b:1}}")]
        public void TODO_PathAsString(string src, string expect) {
            Assert.Equal(Sigo.Parse(expect), Sigo.Parse(src));
        }
    }

    public class SigoWraper {
        public SigoWraper(ISigo sigo) {
            Sigo = sigo;
        }

        public ISigo Sigo { get; }

        public override string ToString() {
            return Sigo.ToString();
        }

        public override bool Equals(object obj) {
            return obj is SigoWraper other && Sigo.Equals(other.Sigo);
        }

        public override int GetHashCode() {
            return Sigo.GetHashCode();
        }
    }
}