using CZero.Lexical.Tokens;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

// https://c0.karenia.cc/c0/token.html

namespace CZero.Lexical
{
    public class Lexer
    {
        private SourceReader _reader { get; set; }

        public Lexer(TextReader sourceCode)
        {
            _reader = new SourceReader(sourceCode);
            var a = new Regex("asd");
            var ss = new Memory<char>();
        }

        public IEnumerable<Token> Parse()
        {
            throw new NotImplementedException();
        }
    }
}
