using Sigobase.Utils;

namespace Sigobase.Database {
    internal class SigoTree : ReadOnlyDictionary, ISigo {
        public SigoTree(int flags, Dict dict) : base(dict) {
            Flags = flags;
#if TESTMODE
            Info = TestInfo.CreateInfo(this);
#endif
        }

        public SigoTree(int flags) : this(flags, new Dict()) { }

        public int Flags { get; private set; }

        public object Data => null;

        public ISigo Get1(string key) {
            Paths.CheckKey(key);
            return TryGetValue(key, out var value) ? value : Sigo.Create(Bits.Def(Flags));
        }

        private ISigo Delete(int rf, string key) {
            rf = Bits.CountDown(rf);
            if (Bits.IsEmpty(rf)) {
                return Sigo.Create(Bits.Proton(rf));
            }

            if (Bits.IsFrozen(rf)) {
                return new SigoTree(Bits.RemoveFrozen(rf), DictCloneRemove(key));
            } else {
                Flags = rf;
                DictRemove(key);
                return this;
            }
        }

        private ISigo Change(int rf, string key, ISigo value) {
            if (Bits.IsFrozen(rf)) {
                return new SigoTree(Bits.RemoveFrozen(rf), DictCloneSet(key, value));
            } else {
                Flags = rf;
                DictSet(key, value);
                return this;
            }
        }

        private ISigo SetFlags(int rf) {
            if (rf == Flags) {
                return this;
            }

            if (Bits.IsEmpty(rf)) {
                return Sigo.Create(Bits.Proton(rf));
            }

            if (Bits.IsFrozen(rf)) {
                return new SigoTree(Bits.RemoveFrozen(rf), DictClone());
            } else {
                Flags = rf;
                return this;
            }
        }

        private ISigo Add(int rf, string key, ISigo value) {
            rf = Bits.CountUp(rf);
            if (Bits.IsFrozen(rf)) {
                return new SigoTree(Bits.RemoveFrozen(rf), DictCloneAdd(key, value));
            } else {
                Flags = rf;
                DictAdd(key, value);
                return this;
            }
        }

        public ISigo Set1(string key, ISigo value) {
            if (TryGetValue(key, out var old)) {
                if (value.Same(old)) {
                    return this;
                }

                var rf = Flags;
                var vf = value.Flags;
                rf = Bits.LeftEffect(rf, vf);

                if (Bits.IsDef(rf, vf)) {
                    return Delete(rf, key);
                } else {
                    return Change(rf, key, value);
                }
            } else {
                var rf = Flags;
                var vf = value.Flags;
                rf = Bits.LeftEffect(rf, vf);

                if (Bits.IsDef(rf, vf)) {
                    return SetFlags(rf);
                } else {
                    return Add(rf, key, value);
                }
            }
        }

        public override string ToString() {
            return this.ToString(Writer.Default);
        }

        public ISigo Freeze() {
            if (Bits.IsFrozen(Flags)) {
                return this;
            }

            foreach (var e in Values) {
                e.Freeze();
            }

            Flags = Bits.AddFrozen(Flags);

            return this;
        }

        public bool Equals(ISigo other) {
            return Sigo.Equals(this, other);
        }

        public override bool Equals(object obj) {
            return obj is ISigo other && Sigo.Equals(this, other);
        }

        public override int GetHashCode() {
            return Sigo.GetHashCode(this);
        }

#if TESTMODE
        public TestInfo Info { get; }
#endif
    }
}