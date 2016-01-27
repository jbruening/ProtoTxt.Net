using System.Text.RegularExpressions;

namespace ProtoTxt
{
    abstract class TokenDefinition
    {
        public bool IsIgnored { get; private set; }
        public Regex Regex { get; private set; }

        protected TokenDefinition(string regex, bool isIgnored = false)
        {
            Regex = new Regex(regex, RegexOptions.Compiled);
            IsIgnored = isIgnored;
        }
    }

    class Whitespace : TokenDefinition
    {
        public Whitespace() : base(@"\s", true) { }
    }

    class NameToken : TokenDefinition
    {
        public NameToken() : base(@"[a-zA-Z_\-]+") { }
    }

    class StringToken : TokenDefinition
    {
        //http://stackoverflow.com/a/10786066
        public StringToken() : base(@"""([^""\\]*(\\.[^""\\]*)*)""") { }
    }

    class NumberToken : TokenDefinition
    {
        //http://www.regular-expressions.info/floatingpoint.html
        public NumberToken() : base(@"[-+]?[0-9]*\.?[0-9]+") { }
    }

    class Obr : TokenDefinition
    {
        public Obr() : base("{") { }
    }

    class Cbr : TokenDefinition
    {
        public Cbr() : base("}") { }
    }

    class Colon : TokenDefinition
    {
        public Colon() : base(":") { }
    }
}
