namespace Sigobase.Utils {
    /// <summary>
    /// Flags constants and methods
    /// </summary>
    internal static class Bits {
        // there are N flag bits, the rest bits are the count of children
        public const int N = 8;

        /// <summary>
        /// The mask of count part
        /// </summary>
        public const int C = -(1 << N);

        /// <summary>
        /// Neutron solid
        /// </summary>
        public const int S = 64;

        /// <summary>
        /// Neutron owned
        /// </summary>
        public const int O = 32;

        /// <summary>
        /// Frozen, the sigo is immutable
        /// </summary>
        public const int F = 16;

        /// <summary>
        /// Neutron P, which indicates that the sigo is a leaf
        /// </summary>
        public const int P = 8;

        /// <summary>
        /// Proton L
        /// </summary>
        public const int L = 4;

        /// <summary>
        /// Proton M
        /// </summary>
        public const int M = 2;

        /// <summary>
        /// Proton R
        /// </summary>
        public const int R = 1;

        /// <summary>
        /// Left effect protons
        /// </summary>
        public const int LM = L + M;

        /// <summary>
        /// Right effect protons
        /// </summary>
        public const int MR = M + R;

        /// <summary>
        /// All protons
        /// </summary>
        public const int LMR = L + M + R;

        /// <summary>
        /// Leaf's flags
        /// </summary>
        public const int FPLMR = F + P + LMR;

        /// <summary>
        /// The bits that can make sigos different
        /// </summary>
        public const int CPLMR = C + P + LMR;

        /// <summary>
        /// No children
        /// </summary>
        public static bool IsEmpty(int f) {
            return (f & C) == 0;
        }

        public static bool IsTree(int f) {
            return (f & P) == 0;
        }

        public static bool IsLeaf(int f) {
            return (f & P) != 0;
        }

        public static bool IsFrozen(int f) {
            return (f & F) != 0;
        }

        public static int RemoveFrozen(int f) {
            return f & ~F;
        }

        public static int AddFrozen(int f) {
            return f | F;
        }

        /// <summary>
        /// Check if we should really add the child to parent
        /// </summary>
        public static bool IsDef(int pf, int cf) {
            return Def(pf) == (cf & (C + P + MR));
        }

        /// <summary>
        /// Same count, same kind{leaf|tree}, same children count 
        /// </summary>
        public static bool IsSame(int fa, int fb) {
            return ((fa ^ fb) & (C + P + LMR)) == 0;
        }

        /// <summary>
        /// Get protons bits
        /// </summary>
        public static int Proton(int f) {
            return f & LMR;
        }

        /// <summary>
        /// Children can change parent flags
        /// </summary>
        public static int LeftEffect(int pf, int cf) {
            // issue: add proton L
            return (cf & L) != 0 ? pf | LM : pf;
        }

        /// <summary>
        /// Children count
        /// </summary>
        public static int Count(int f) {
            return f >> N;
        }

        /// <summary>
        /// count++
        /// </summary>
        public static int CountUp(int f) {
            return f - C;
        }

        /// <summary>
        /// count--
        /// </summary>
        public static int CountDown(int f) {
            return f + C;
        }

        /// <summary>
        /// Default children of sigos
        /// </summary>
        public static int Def(int f) {
            return (f & R) != 0 ? MR : 0;
        }

        public static bool HasM(int f) {
            return (f & M) != 0;
        }

        public static bool HasR(int f) {
            return (f & R) != 0;
        }
    }
}