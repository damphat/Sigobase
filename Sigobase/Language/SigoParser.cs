using Sigobase.Database;

namespace Sigobase.Language {

    // abstract Parser, also a factory
    public abstract class SigoParser {
        public static SigoParser Create(string src) {
            return new SigoParserV2(src);
        }

        public abstract ISigo Parse();
        public abstract ISigo Parse(ISigo input, out ISigo output);
    }
}