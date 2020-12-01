using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using System.Text;

namespace CZero.Lexical.Tokens
{
    public class DoubleLiteralToken : LiteralToken
    {
        public double Value { get; }

        public DoubleLiteralToken(double value, SourcePosition position) : base(position)
        {
            Guard.Against.Negative(value, nameof(value));
            Value = value;
        }
    }
}
