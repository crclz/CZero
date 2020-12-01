using System;
using System.Collections.Generic;
using System.Text;

namespace CZero.Lexical.Tokens
{
    public class EOFToken : Token
    {
        public EOFToken(SourcePosition position) : base(position)
        {
        }
    }
}
