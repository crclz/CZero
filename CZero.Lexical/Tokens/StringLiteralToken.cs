using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using System.Text;

namespace CZero.Lexical.Tokens
{
    public class StringLiteralToken : LiteralToken
    {
        public string Value { get; }

        public StringLiteralToken(string value, SourcePosition position) : base(position)
        {
            Guard.Against.Null(value, nameof(value));

            Value = value;
        }
    }
}
