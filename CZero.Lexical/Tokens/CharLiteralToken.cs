using System;
using System.Collections.Generic;
using System.Text;

namespace CZero.Lexical.Tokens
{
    public class CharLiteralToken : LiteralToken
    {
        public char Value { get; }

        public CharLiteralToken(char value, SourcePosition position) : base(position)
        {
            Value = value;
        }
    }
}
