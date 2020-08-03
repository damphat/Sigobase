using System.Globalization;

namespace Sigobase.Language.Utils {
    public static class SigoConverter {
        // FIXME ToDouble("1e1000") return Inf for Net Core, throw exception .NET Framework
        public static double ToDouble(string str) {
            return double.Parse(str, CultureInfo.InvariantCulture);
        }
    }
}