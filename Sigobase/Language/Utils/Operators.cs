using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Sigobase.Language.Utils {
    internal static class Operators {
        public static double Unary(Kind kind, object value) {
            switch (kind) {
                case Kind.Plus: return +ToNumber(value);
                case Kind.Minus: return -ToNumber(value);
                default: throw new NotImplementedException();
            }
        }

        public static object Binary(Kind kind, object a, object b) {
            switch (kind) {
                case Kind.Plus: {
                    if (a is string sa) {
                        return sa + ToStr(b);
                    } else if (b is string sb) {
                        return ToStr(a) + sb;
                    } else {
                        return ToNumber(a) + ToNumber(b);
                    }
                }
                case Kind.Minus: return ToNumber(a) - ToNumber(b);
                case Kind.Mul: return ToNumber(a) * ToNumber(b);
                case Kind.Div: return ToNumber(a) / ToNumber(b);
                default: throw new NotImplementedException();
            }
        }

        public static double ToNumber(string s) {
            if (string.IsNullOrWhiteSpace(s)) return 0;

            try {
                return double.Parse(s, CultureInfo.InvariantCulture);
            } catch (FormatException) {
                return double.NaN;
            } catch (OverflowException) {
                return Regex.IsMatch(s, "^\\s-") ? double.NegativeInfinity : double.PositiveInfinity;
            }
        }

        public static double ToNumber(object value) {
            switch (value) {
                case null: return 0;
                case bool b: return b ? 1 : 0;
                case double d: return d;
                case string s: return ToNumber(s);
                default: throw new NotImplementedException();
            }
        }

        public static string ToStr(bool value) {
            return value ? "true" : "false";
        }

        public static string ToStr(double value) {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public static string ToStr(object value) {
            switch (value) {
                case null: return "null";
                case bool b: return b ? "true" : "false";
                case double d: return d.ToString(CultureInfo.InvariantCulture);
                case string s: return s;
                default: throw new NotImplementedException();
            }
        }
    }
}
