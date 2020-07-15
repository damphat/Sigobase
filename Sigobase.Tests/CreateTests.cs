using Sigobase.Database;
using Xunit;

namespace Sigobase.Tests {
    public class CreateTests {
        [Fact]
        public void WithEmptyPath_return_the_value() {
            var v = Sigo.From("v");

            Assert.Equal(v, Sigo.Create(3, null, v));
            Assert.Equal(v, Sigo.Create(3, "", v));
            Assert.Equal(v, Sigo.Create(3, "/", v));
        }

        [Fact]
        public void WithPath() {
            var user = Sigo.Create(3,
                "name/first", "Phat",
                "name/last", "Dam",
                "male", true);

            Assert.Equal("Phat", user.Get1("name").Get1("first").Data);
            Assert.Equal("Dam", user.Get1("name").Get1("last").Data);
            Assert.Equal(true, user.Get1("male").Data);
        }

        [Fact]
        public void Allow_duplicated_path_the_last_is_used() {
            var user = Sigo.Create(3,
                "user/id", 100.0,
                "user/id", 200.0,
                "user/id", 200.0);

            Assert.Equal(200.0, user.Get1("user").Get1("id").Data);
        }

        [Fact]
        public void Path_can_be_any_types_convertable_to_string() {
            var paths = new object[] {true, false, 1.5, 'c'};
            foreach (var path in paths) {
                var sigo = Sigo.Create(3, path, "v");

                Assert.Equal("v", sigo.Get1(path.ToString()).Data);
            }
        }
    }
}