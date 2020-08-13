namespace Sigobase.Language {
    public class TokenError {
        public TokenErrorKind Kind { get; }
        public int At { get; }

        public TokenError(TokenErrorKind kind, int at) {
            this.Kind = kind;
            this.At = at;
        }

    }
}
