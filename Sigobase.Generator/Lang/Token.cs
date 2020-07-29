namespace Sigobase.Generator.Lang {
    internal class Token {
        private string raw;

        public Token(Kind kind, string src, int start, int end, int separator, object value = null) {
            Kind = kind;
            Src = src;
            Start = start;
            End = end;
            Separator = separator;
            Value = value;
        }

        public string Src { get; }
        public int End { get; }
        public int Start { get; }

        public Kind Kind { get; }

        public int Separator { get; }
        public string Raw => raw ??= Src.Substring(Start, End - Start);

        public object Value { get; }

        public override string ToString() {
            return $"{Kind}:'{Raw}'";
        }
    }
}