using CZero.Lexical.Tokens;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

// https://c0.karenia.cc/c0/token.html

namespace CZero.Lexical
{
    public class Lexer
    {
        private SourceReader _reader { get; set; }

        public Lexer(TextReader sourceCode)
        {
            _reader = new SourceReader(sourceCode);
        }

        public IEnumerable<Token> Parse()
        {
            throw new NotImplementedException();
        }
    }
}
