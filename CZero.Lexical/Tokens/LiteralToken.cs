using System;
using System.Collections.Generic;
using System.Text;

namespace CZero.Lexical.Tokens
{
    public abstract class LiteralToken : Token
    {
        protected LiteralToken(SourcePosition position) : base(position)
        {
        }
    }
}
