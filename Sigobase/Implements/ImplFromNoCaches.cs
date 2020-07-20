using System;
using System.Collections;
using System.Globalization;
using Sigobase.Database;

namespace Sigobase.Implements {
    public static class ImplFromNoCaches {
        public static ISigo From(bool b) {
            return new SigoLeaf(b);
        }

        public static ISigo From(double d) {
            return new SigoLeaf(d);
        }

        public static ISigo From(string s) {
            return new SigoLeaf(s);
        }

        public static ISigo From(IEnumerable list) {
            var sigo = Sigo.Create(3);
            var i = 0;
            foreach (var e in list) {
                sigo = sigo.Set1(i.ToString(CultureInfo.InvariantCulture), From(e));
                i++;
            }

            return sigo;
        }

        public static ISigo From(IDictionary dict) {
            var sigo = Sigo.Create(3);
            foreach (DictionaryEntry e in dict) {
                sigo = sigo.Set(e.Key.ToString(), From(e.Value));
            }

            return sigo;
        }

        public static ISigo From(object o) {
            switch (o) {
                case null:
                    return Sigo.Create(3);
                case ISigo sigo:
                    return sigo;
                case bool b:
                    return From(b);
                case int _:
                case long _:
                case float _:
                    return From(Convert.ToDouble(o));
                case double d:
                    return From(d);
                case string s:
                    return From(s);
                case IDictionary dict:
                    return From(dict);
                case IEnumerable list:
                    return From(list);
                default:
                    throw new NotImplementedException($"{o.GetType().Name}");
            }
        }
    }
}