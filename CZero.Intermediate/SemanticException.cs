using System;
using System.Collections.Generic;
using System.Text;

namespace CZero.Intermediate
{
    class SemanticException : Exception
    {
        public SemanticException(string message) : base(message)
        {
        }
    }
}
