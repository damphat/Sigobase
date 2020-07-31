using Sigobase.Utils;

namespace Sigobase.Database {
    internal class SigoLeaf : EmptyReadOnlyDictionary, ISigo {
        public int Flags => Bits.FPLMR;

        public SigoLeaf(object data) {
            Data = data;
#if TESTMODE
            Info = TestInfo.CreateInfo(this);
#endif
        }

        public ISigo Get1(string key) {
            Paths.CheckKey(key);
            return Sigo.Create(Bits.MR);
        }

        public ISigo Set1(string key, ISigo value) {
            switch (value.Flags & Bits.CPLMR) {
                case Bits.MR: return this;
                case Bits.LMR: return Sigo.Create(Bits.LMR);
                default: return Sigo.Create(Bits.LMR).Set1(key, value);
            }
        }

        public object Data { get; }

        public override string ToString() {
            return this.ToString(Writer.Default);
        }

        public ISigo Freeze() {
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