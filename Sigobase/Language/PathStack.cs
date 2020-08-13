using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Sigobase.Utils;

namespace Sigobase.Language {
    public class PathStack : IReadOnlyList<string> {
        private List<string> list = new List<string>();
        private List<int> stack = new List<int>();

        public PathStack() { }

        public PathStack(string src) {
            var paths = src.Split(':');
            var first = true;
            foreach (var path in paths) {
                if (first) {
                    first = false;
                } else {
                    Push();
                }

                Add(path);
            }
        }

        public override bool Equals(object obj) {
            if (obj is PathStack ps) {
                return Enumerable.SequenceEqual(list, ps.list)
                       && Enumerable.SequenceEqual(stack, ps.stack);
            }

            return false;
        }

        public int Start { get; private set; }
        public int End { get; private set; }

        public void Add1(string key) {
            list.Add(key);
            End++;
        }

        public void Add(string path) {
            if (string.IsNullOrEmpty(path)) {
                return;
            }

            if (Paths.ShouldSplit(path)) {
                var keys = Paths.Split(path);
                list.AddRange(keys);
                End += keys.Length;
            } else {
                Add1(path);
            }
        }

        public void Add(object key) {
            switch (key) {
                case null:
                    Add1("null");
                    break;
                case bool b:
                    Add1(b ? "true" : "false");
                    break;
                case string path:
                    Add(path);
                    break;
                case double d:
                    Add1(d.ToString(CultureInfo.InvariantCulture));
                    break;
                default:
                    Add1(Convert.ToString(key, CultureInfo.InvariantCulture));
                    break;
            }
        }

        public void Push() {
            stack.Add(End);
            Start = End;
        }

        public PathStack Pop() {
            if (stack.Count == 0) {
                throw new Exception("No thing to pop");
            }

            End = Start;
            list.RemoveRange(End, list.Count - End);
            stack.RemoveAt(stack.Count - 1);

            if (stack.Count > 0) {
                Start = stack[stack.Count - 1];
            } else {
                Start = 0;
            }

            return this;
        }

        public void Clear() {
            if (End > Start) {
                End = Start;
                list.RemoveRange(End, list.Count - End);
            }
        }

        public IEnumerator<string> GetEnumerator() {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public int Count => list.Count;

        public string this[int index] => list[index];

        // Debug String
        public override string ToString() {
            var sb = new StringBuilder();
            var start = 0;
            var first0 = true;
            foreach (var end in stack) {
                if (first0) {
                    first0 = false;
                } else {
                    sb.Append(":");
                }

                var first1 = true;
                for (var i = start; i < end; i++) {
                    if (first1) {
                        first1 = false;
                    } else {
                        sb.Append('/');
                    }

                    sb.Append(list[i]);
                }

                start = end;
            }

            if (!first0) {
                sb.Append(":");
            }

            first0 = true;
            for (var i = Start; i < End; i++) {
                if (first0) {
                    first0 = false;
                } else {
                    sb.Append('/');
                }

                sb.Append(list[i]);
            }

            return sb.ToString();
        }

        public override int GetHashCode() {
            throw new NotImplementedException("")
        }
    }
}