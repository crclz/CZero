using System;
using System.Collections.Generic;
using System.Text;

namespace CZero.Lexical.Tokens
{
    public class UInt64LiteralToken : LiteralToken
    {
        public ulong Value { get; }

        public UInt64LiteralToken(ulong value, SourcePosition position) : base(position)
        {
            Value = value;
        }
    }
}
