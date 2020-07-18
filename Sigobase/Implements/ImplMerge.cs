using System;
using Sigobase.Database;
using Sigobase.Utils;

namespace Sigobase.Implements {
    public static class ImplMerge {
        public static ISigo UpLM(ISigo a, int f) {
            if (f != (f & 6)) {
                throw new ArgumentException("only L, M protons accepted");
            }

            if (a.Flags == (a.Flags | f)) {
                return a;
            }

            // {4, x: {3}} * {6} => {6|4, x:{3}}
            var ret = Sigo.Create(a.Flags | f);
            foreach (var e in a) {
                ret.Set1(e.Key, e.Value);
            }

            return ret;
        }

        public static ISigo Solid(int lm1, ISigo b) {
            // {3} * {0, x:{3}}
            ISigo ret = null;

            // new if b not frozen or miss some lm1
            if (16 + lm1 != ((16 + lm1) & b.Flags)) {
                ret = Sigo.Create(7 & (lm1 | b.Flags));
            }

            foreach (var e in b) {
                var k = e.Key;
                var bk = e.Value;
                var rk = Solid(3, bk);
                if (ret != null) {
                    ret = ret.Set1(k, rk);
                } else {
                    if (ReferenceEquals(rk, bk)) {
                        continue;
                    }

                    ret = Sigo.Create(7 & (lm1 | b.Flags));
                    foreach (var t in b) {
                        if (t.Key != k) {
                            ret = ret.Set1(t.Key, t.Value);
                        } else {
                            ret = ret.Set1(k, rk);
                            break;
                        }
                    }
                }
            }

            return ret ?? b;
        }

        public static ISigo Merge(int fa, ISigo b) {
            return null;
        }

        public static ISigo Merge(ISigo a, int fb) {
            return null;
        }

        public static ISigo Merge(ISigo a, ISigo b) {
            if (ReferenceEquals(a, b)) {
                // test: same
                return a;
            }

            var fa = a.Flags;
            var fb = b.Flags;

            #region leaf

            if (Bits.IsLeaf(fb)) {
                if (Bits.IsLeaf(fa)) {
                    // test: PP_return_a_if_same
                    return Equals(a.Data, b.Data) ? a : b;
                }

                // test: SP_return_b_if_not_same
                return b;
            }

            if (Bits.IsLeaf(fa)) {
                if (Bits.HasM(fb)) {
                    return Merge(7, b);
                } else {
                    return a;
                }
            }

            #endregion leaf

            if (fb < 256) {
                if (fa < 256) {
                    // test: EE_return_E
                    return Sigo.Create(7 & (fa | fb));
                } else {
                    if (Bits.HasR(fb)) {
                        // {6, x:1} * {1}
                        // test: NE1_return_E
                        return Sigo.Create(7 & (fa | fb));
                    } else {
                        // {6, x:1} * {0}
                        // {0, x:{3}} * {2}

                        // test: NE0_return_a_if_frozen_and_flag_unchanged
                        fb = (fb & 7) + 16;
                        if (fb == (fa & fb)) {
                            return a;
                        }

                        // test: NE0_return_new_if_a_is_not_frozen
                        // test: NE0_return_new_if_r_flag_change
                        var ret = Sigo.Create(7 & (fa | fb));

                        foreach (var e in a) {
                            ret = ret.Set1(e.Key, e.Value.Clone());
                        }

                        return ret;
                    }
                }
            } else {
                // *N
                if (fa < 256) {
                    if (Bits.HasR(fa)) {
                        // {1} * {x:1}
                        // test: E0
                    } else {
                        // {0} * {x: {0}}
                    }
                } else {
                    if (Bits.HasR(fb)) {
                        // {7, x:1} * {6, y:2}
                    } else {
                        // {7 x:1} * {3, x:{0}}
                    }
                }
            }

            throw new NotImplementedException($"{a} * {b}");
        }
    }
}