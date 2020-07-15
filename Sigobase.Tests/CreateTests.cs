using Sigobase.Database;
using Xunit;

namespace Sigobase.Tests {
    public class CreateTests {
        [Fact]
        public void AllowPathWithoutKeys_returnTheChild() {
            // TODO should we disallow empty paths?

            var child = Sigo.From("child");

            Assert.Equal(child, Sigo.Create(3, null, child));
            Assert.Equal(child, Sigo.Create(3, "", child));
            Assert.Equal(child, Sigo.Create(3, "/", child));
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
            // TODO should we disallow overwriting
            var user = Sigo.Create(3,
                "user/id", 100.0,
                "user/id", 200.0,
                "user/id", 200.0);

            Assert.Equal(200.0, user.Get1("user").Get1("id").Data);
        }

        [Fact]
        public void AllowPathFromObject() {
            // TODO object to path specification
            // TODO in strict mode, only non empty strings and integers are allows 
            var paths = new object[] {true, false, 1.5, 'c'};
            foreach (var path in paths) {
                var sigo = Sigo.Create(3, path, "v");

                Assert.Equal("v", sigo.Get1(path.ToString()).Data);
            }
        }
    }
}