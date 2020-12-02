using System;
using System.Collections.Generic;
using System.Text;

namespace CZero.Lexical
{
    public class LexerException : Exception
    {
        public LexerException(string message) : base(message)
        {
        }
    }
}
