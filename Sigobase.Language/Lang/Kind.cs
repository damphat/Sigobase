namespace Sigobase.Language.Lang {
    public enum Kind {
        Number,
        String,
        Identifier,

        Open,
        Close,
        Colon,
        Comma,
        SemiColon,

        Or,
        Div,
        Question,

        Eq,

        Unknown,
        Eof
    }
}