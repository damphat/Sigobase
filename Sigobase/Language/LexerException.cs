using System;
using System.Runtime.Serialization;

namespace Sigobase.Language {
    [Serializable]
    public class LexerException : Exception {
        public LexerException() { }
        protected LexerException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public LexerException(string message) : base(message) { }
        public LexerException(string message, Exception innerException) : base(message, innerException) { }
    }
}