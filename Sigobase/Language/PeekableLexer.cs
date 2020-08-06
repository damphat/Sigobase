using System;
using System.Text;

namespace Sigobase.Language {
    public class PeekableLexer {
        private readonly Lexer lexer;
        private readonly Token[] buffer;
        public int Min { get; }
        public int Max { get; }
        public int Cursor { get; private set; }
        private int readCount;

        public PeekableLexer(string src, int min, int max) {
            lexer = new Lexer(src);
            this.Min = min;
            this.Max = max;
            buffer = new Token[max - min + 1];
        }

        private void Next() {
            var i = readCount % buffer.Length;
            buffer[i] = lexer.Read(buffer[i]);
            readCount++;
        }

        public Token Peek(int delta) {
            if (delta < Min || delta > Max) {
                throw new ArgumentOutOfRangeException(nameof(delta));
            }

            var c = Cursor + delta;
            if (c < 0) {
                throw new ArgumentOutOfRangeException(nameof(delta), "cursor + delta < 0");
            }

            if (c < readCount - buffer.Length) {
                throw new ArgumentOutOfRangeException(nameof(delta), "cursor + delta < readCount - bufferLength");
            }

            while (readCount <= c) {
                Next();
            }

            return buffer[c % buffer.Length];
        }

        public void Move(int delta) {
            var c = Cursor + delta;
            if (c < 0) {
                throw new ArgumentOutOfRangeException(nameof(delta), "cursor + delta < 0");
            }

            if (c < readCount - buffer.Length) {
                throw new ArgumentOutOfRangeException(nameof(delta), "cursor + delta < readCount - bufferLength");
            }

            Cursor = c;
        }

        public override string ToString() {
            var sb = new StringBuilder();
            sb.Append('[');
            for (var i = readCount - buffer.Length; i < readCount; i++) {
                if (i < 0) {
                    sb.Append("null");
                } else {
                    if (i == Cursor) {
                        sb.Append('(');
                    }

                    sb.Append(i);
                    sb.Append(':');
                    sb.Append(buffer[i % buffer.Length].Raw);
                    if (i == Cursor) {
                        sb.Append(')');
                    }
                }

                sb.Append(' ');
            }

            sb.Append(']');

            if (Cursor >= readCount) {
                sb.Append('(').Append(Cursor).Append(')');
            }

            return sb.ToString();
        }
    }
}