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

        /// <returns>When read EOF, return 0</returns>
        public char Peek()
        {
            var x = _reader.Peek();
            if (x == -1)
            {
                return '\0';
            }

            return (char)x;
        }

        /// <returns>When read EOF, return 0</returns>
        public char Next()
        {
            var x = _reader.Read();
            if (x == -1)
            {
                return '\0';
            }

            char c = (char)x;

            // Change position pointers

            PreviousPosition = Position;

            if (c == '\n')
            {
                Position = Position.StartOfNextLine();
            }
            else
            {
                Position = Position.NextCloumn();
            }

            return c;
        }

        /// <returns>When read EOF, return 0</returns>
        public char NextNonWhiteChar()
        {
            while (true)
            {
                char c;
                if ((c = Next()) != '\0')
                {
                    if (!char.IsWhiteSpace(c))
                    {
                        return c;
                    }
                }
                else
                {
                    // EOF
                    return '\0';
                }
            }

        }
    }
}
