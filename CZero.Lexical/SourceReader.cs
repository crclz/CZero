using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("CZero.Lexical.Test")]
namespace CZero.Lexical
{
    class SourceReader
    {
        private TextReader _reader { get; }

        public SourcePosition PreviousPosition { get; private set; }

        /// <summary>
        /// Points to next char to read
        /// </summary>
        public SourcePosition Position { get; private set; } = new SourcePosition(0, 0);

        public SourceReader(TextReader reader)
        {
            _reader = reader;
        }

        public bool TryPeek(out char c)
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

        public char Peek()
        {
            if (!TryPeek(out char c))
                throw new InvalidOperationException();
            return c;
        }

        public bool TryNext(out char c)
        {
            var x = _reader.Read();
            if (x == -1)
            {
                c = '\0';
                return false;
            }

            c = (char)x;

            // Change position pointers

            PreviousPosition = Position;

            if (c == '\n')
            {
                Position = Position.NextCloumn();
            }
            else
            {
                Position = Position.StartOfNextLine();
            }

            return true;
        }

        public char Next()
        {
            if (!TryNext(out char c))
                throw new InvalidOperationException();
            return c;
        }

        public bool NextNonWhiteChar(out char nonWhiteChar)
        {
            while (true)
            {
                if (TryNext(out char c))
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
