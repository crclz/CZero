using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace CZero.Lexical
{
    public class SourceStringReader
    {
        public string SourceCode { get; }

        /// <summary> Points to next char in the SourceCode </summary>
        public int Cursor { get; private set; } = 0;

        /// <summary> Points to next char in the SourceCode </summary>
        public SourcePosition Position { get; private set; } = new SourcePosition(0, 0);

        public SourcePosition PreviousPosition { get; private set; }

        public SourceStringReader(string sourceCode)
        {
            SourceCode = Guard.Against.Null(sourceCode, nameof(sourceCode));
        }

        public bool ReachedEnd => Cursor == SourceCode.Length;

        public char CurrentChar()
        {
            if (ReachedEnd)
                throw new InvalidOperationException();

            return SourceCode[Cursor];
        }

        public void Advance()
        {
            if (ReachedEnd)
                throw new InvalidOperationException();

            var c = CurrentChar();

            Cursor++;

            PreviousPosition = Position;

            if (c == '\n')
            {
                Position = Position.StartOfNextLine();
            }
            else
            {
                Position = Position.NextCloumn();
            }
        }

        public void Advance(int count)
        {
            Guard.Against.NegativeOrZero(count, nameof(count));

            for (var i = 0; i < count; i++)
                Advance();
        }

        /// <returns> If reached end, false is returned </returns>
        public bool AdvanceUntilNonWhite()
        {
            while (!ReachedEnd)
            {
                if (!char.IsWhiteSpace(CurrentChar()))
                    return true;

                Advance();
            }

            return false;
        }
    }
}
