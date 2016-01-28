using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ProtoTxt
{
    class Parser
    {
        public static List<Token> Tokenize(ref string source)
        {
            var ci = 0;
            var cl = 1;
            var cc = 0;
            var eolr = new Regex("(\r\n|\r|\n)");

            var tokens = new List<Token>();

            var tokenDefinitions = new TokenDefinition[]
            {
                new StringToken(),
                new NameToken(),
                new NumberToken(), 
                new Obr(),
                new Cbr(),
                new Colon(), 
                new Whitespace(),
                new CommentToken(), 
            };
            
            while (ci < source.Length)
            {
                TokenDefinition md = null;
                var ml = 0;

                foreach (var rule in tokenDefinitions)
                {
                    var match = rule.Regex.Match(source, ci);

                    if (match.Success && (match.Index - ci) == 0)
                    {
                        md = rule;
                        ml = match.Length;
                        break;
                    }
                }

                if (md == null)
                    throw new Exception(string.Format("Unrecognized symbol '{0}' at index {1} (line {2}, column {3}).",
                        source[ci], ci, cl, cc));

                var value = source.Substring(ci, ml);
                if (!md.IsIgnored)
                    tokens.Add(new Token(md.GetType(), ci, ml, cl, cc));

                var eol = eolr.Match(value);
                if (eol.Success)
                {
                    cl++;
                    cc = value.Length - (eol.Index + eol.Length);
                }
                else
                {
                    cc += ml;
                }

                ci += ml;
            }

            return tokens;
        }
    }
}