using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using System.Text;

namespace CZero.Lexical.Tokens
{
    public abstract class Token
    {
        public SourcePosition SourcePosition { get; }

        public Token(SourcePosition position)
        {
            Guard.Against.Null(position, nameof(position));
        }
    }
}
