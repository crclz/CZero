using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CZero.Lexical
{
    class SourceReader
    {
        private TextReader _reader { get; }

        private SourcePosition _position { get; set; } = new SourcePosition(0, 0);

        public SourceReader(TextReader reader)
        {
            _reader = reader;
        }

        public bool Peek(out char c)
        {
            var x = _reader.Peek();
            if (x == -1)
            {
                c = '\0';
                return false;
            }

            c = (char)x;
            return true;
        }

        public bool Next(out char c)
        {
            var x = _reader.Read();
            if (x == -1)
            {
                c = '\0';
                return false;
            }

            c = (char)x;

            // Change the line and column counter
            if (c == '\n')
            {
                _position = _position.NextCloumn();
            }
            else
            {
                _position = _position.StartOfNextLine();
            }

            return true;
        }

        public bool NextNonWhiteChar(out char nonWhiteChar)
        {
            while (true)
            {
                if (Next(out char c))
                {
                    if (!char.IsWhiteSpace(c))
                    {
                        nonWhiteChar = c;
                        return true;
                    }
                }
                else
                {
                    // EOF
                    nonWhiteChar = '\0';
                    return false;
                }
            }

        }
    }
}
