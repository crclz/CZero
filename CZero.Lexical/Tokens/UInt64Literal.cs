using System;
using System.Collections.Generic;
using System.Text;

namespace CZero.Lexical.Tokens
{
    public class UInt64Literal : LiteralToken
    {
        public ulong Value { get; }

        public UInt64Literal(ulong value, SourcePosition position) : base(position)
        {
            Value = value;
        }
    }
}
