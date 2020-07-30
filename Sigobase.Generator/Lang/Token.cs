namespace Sigobase.Generator.Lang {
    public class Token {
        private string raw;

        public Token(Kind kind, string src, int start, int end, int separator, object value = null) {
            Kind = kind;
            Src = src;
            Start = start;
            End = end;
            Separator = separator;
            Value = value;
        }

        public Kind Kind { get; private set; }
        public string Src { get; private set; }
        public int Start { get; private set; }
        public int End { get; private set; }
        public int Separator { get; private set; }

        public string Raw => raw = raw ?? Src.Substring(Start, End - Start);

        public object Value { get; private set; }

        public override string ToString() {
            return $"{Kind}:'{Raw}'";
        }

        public void Reset(Kind kind, string src, in int start, in int end, in int separator, object value = null) {
            Kind = kind;
            Src = src;
            Start = start;
            End = end;
            Separator = separator;
            Value = value;
            raw = null;
        }
    }
}