using CZero.Lexical.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace CZero.Syntactic
{
    public partial class SyntacticAnalyzer
    {
        private readonly TokenReader _reader;

        public SyntacticAnalyzer(TokenReader reader)
        {
            _reader = reader;
        }
    }
}
