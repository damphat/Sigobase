namespace Sigobase.Language {
    public enum Kind {
        Number,
        String,
        Identifier,

        Open,
        Close,
        OpenBracket,
        CloseBracket,
        OpenParens,
        CloseParens,

        Colon,
        Comma,
        SemiColon,

        Plus,
        Minus,
        Mul,
        Div,
        Or,
        Question,

        Eq,
        EqEq,
        Not,
        NotEq,

        Unknown,
        Eof,
    }
}