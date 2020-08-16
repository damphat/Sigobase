using Sigobase.Database;
using Xunit;

namespace Sigobase.Tests {
    public class SetTests {
        [Fact]
        public void See_createTests() {
            var user = Sigo.Create(1);
            user = user.Set("name/first", Sigo.From("Phat"));
            user = user.Set("name/last", Sigo.From("Dam"));
            user = user.Set("male", Sigo.From(true));

            SigoAssert.Equal("Phat", user.Get1("name").Get1("first").Data);
            SigoAssert.Equal("Dam", user.Get1("name").Get1("last").Data);
            SigoAssert.Equal(true, user.Get1("male").Data);
        }
    }
}