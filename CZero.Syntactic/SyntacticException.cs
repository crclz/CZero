using System;
using System.Collections.Generic;
using System.Text;

namespace CZero.Syntactic
{
    public class SyntacticException : Exception
    {
        public SyntacticException(string message) : base(message)
        {
        }
    }
}
