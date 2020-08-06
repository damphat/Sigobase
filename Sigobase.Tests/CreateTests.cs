using System;
using Sigobase.Database;
using Xunit;

namespace Sigobase.Tests {
    public class CreateTests {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("/")]
        public void AllowPathWithoutKeys_returnTheChild(string path) {
            var value = Sigo.From("child");
            Assert.Equal(value, Sigo.Create(3, path, value));
        }

        [Theory]
        [InlineData(true, "true")]
        [InlineData(false, "false")]
        [InlineData(0.0, "0")]
        [InlineData(1.5, "1.5")]
        [InlineData(double.PositiveInfinity, "Infinity")]
        public void AllowPathFromObject(object key, string sameAs) {
            var sigo = Sigo.Create(3, key, "v");
            Assert.Equal("v", sigo.Get1(sameAs).Data);
        }

        [Fact]
        public void AllowPath() {
            var user = Sigo.Create(3,
                "name/first", "Phat",
                "name/last", "Dam",
                "male", true);

            Assert.Equal("Phat", user.Get1("name").Get1("first").Data);
            Assert.Equal("Dam", user.Get1("name").Get1("last").Data);
            Assert.Equal(true, user.Get1("male").Data);
        }

        [Fact]
        public void AllowPathDuplicated_overwritingChild() {
            var user = Sigo.Create(3,
                "user/id", 100.0,
                "user/id", 200.0,
                "user/id", 200.0);

            Assert.Equal(200.0, user.Get1("user").Get1("id").Data);
        }

        [Theory(Skip = "TODO")]
        [InlineData(false, null)]
        [InlineData(false, "")]
        [InlineData(false, "/")]
        [InlineData(false, "1.5")]
        [InlineData(false, "/a")]
        [InlineData(true, "1")]
        [InlineData(true, "a")]
        [InlineData(true, "a/1")]
        public void TODO_strict(bool allow, string path) {
            if (allow) {
                Assert.Equal("v", Sigo.Create(3, path, "v").Get(path).Data);
            } else {
                Assert.ThrowsAny<Exception>(() => Sigo.Create(3, path, "v"));
            }
        }
    }
}