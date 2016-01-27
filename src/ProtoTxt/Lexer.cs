using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ProtoTxt
{
    class Lexer
    {
        public static ProtoObject Lex(IEnumerable<Token> tokens, ref string source)
        {
            return LexObject(tokens.GetEnumerator(), false, ref source);
        }

        private static ProtoObject LexObject(IEnumerator<Token> enumerator, bool sub, ref string source)
        {
            var obj = new ProtoObject();
            while (enumerator.MoveNext())
            {
                var token = enumerator.Current;
                if (token.TokenType == typeof(NameToken))
                {
                    var propName = source.Substring(token.Position, token.Length);
                    obj.Properties.Add(LexProp(propName, enumerator, ref source));
                }
                else if (token.TokenType == typeof (Cbr) && sub)
                    return obj;
                else
                    throw UnexpectedTokenType(token, ref source);
            }
            return obj;
        }

        private static ProtoProp LexProp(string name, IEnumerator<Token> enu, ref string source)
        {
            if (!enu.MoveNext())
                throw new Exception("Expected a token");

            var token = enu.Current;
            if (token.TokenType == typeof (Colon))
            {
                return new ProtoProp {Name = name, Value = GetValue(enu, ref source)};
            }
            
            if (token.TokenType == typeof (Obr))
            {
                var ret = new ProtoProp {Name = name, Value = LexObject(enu, true, ref source)};
                return ret;
            }

            throw UnexpectedTokenType(token, ref source);
        }

        private static object GetValue(IEnumerator<Token> enu, ref string source)
        {
            if (!enu.MoveNext())
                throw new Exception("Expected a value token");
            var token = enu.Current;
            if (token.TokenType == typeof (StringToken))
                return source.Substring(token.Position + 1, token.Length - 2);
            
            if (token.TokenType == typeof (NumberToken))
            {
                var val = source.Substring(token.Position, token.Length);
                if (val.Contains("."))
                {
                    float fl;
                    if (float.TryParse(val, out fl))
                        return fl;
                    double dbl;
                    if (double.TryParse(val, out dbl))
                        return dbl;
                    return val;
                }
                int @int;
                if (int.TryParse(val, out @int))
                    return @int;
                return val;
            }

            if (token.TokenType == typeof (NameToken))
            {
                var val = source.Substring(token.Position, token.Length);
                //not sure what these are...
                return val;
            }

            throw UnexpectedTokenType(token, ref source);
        }

        private static Exception UnexpectedTokenType(Token token, ref string source)
        {
            return new Exception(string.Format("Unexpected token {0} ({2}) at line {1}, column {3}",
                        source.Substring(token.Position, token.Length), token.Line, token.TokenType.Name, token.Column));
        }
    }

    [DebuggerDisplay("{Name}: {Value}")]
    public class ProtoProp
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string Name { get; set; }
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public object Value { get; set; }
    }

    public class ProtoObject
    {
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public List<ProtoProp> Properties { get; private set; }
        public ProtoObject() { Properties = new List<ProtoProp>();}
    }
}