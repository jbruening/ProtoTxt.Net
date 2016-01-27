using System;

namespace ProtoTxt
{
    struct Token
    {
        public readonly Type TokenType;
        public readonly int Position;
        public readonly int Length;
        public readonly int Line;
        public readonly int Column;

        public Token(Type tokenType, int position, int length, int line, int column)
            : this()
        {
            TokenType = tokenType;
            Position = position;
            Length = length;
            Line = line;
            Column = column;
        }
    }
}