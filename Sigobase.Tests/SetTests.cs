using Sigobase.Database;
using Xunit;

namespace Sigobase.Tests {
    public class SetTests {
        private ISigo v = Sigo.From("v");

        [Fact]
        public void See_createTests() {
            var user = Sigo.Create(1);
            user = user.Set("name/first", Sigo.From("Phat"));
            user = user.Set("name/last", Sigo.From("Dam"));
            user = user.Set("male", Sigo.From(true));

            Assert.Equal("Phat", user.Get1("name").Get1("first").Data);
            Assert.Equal("Dam", user.Get1("name").Get1("last").Data);
            Assert.Equal(true, user.Get1("male").Data);
        }
    }
}