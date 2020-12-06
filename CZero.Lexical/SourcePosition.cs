using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using System.Text;

namespace CZero.Lexical
{
    public struct SourcePosition
    {
        public int Line { get; }
        public int Column { get; }

        public SourcePosition(int line, int column)
        {
            Guard.Against.Negative(line, nameof(line));
            Guard.Against.Negative(column, nameof(column));

            Line = line;
            Column = column;
        }

        public static implicit operator SourcePosition((int, int) pos)
        {
            return new SourcePosition(pos.Item1, pos.Item2);
        }

        public SourcePosition NextCloumn()
        {
            return new SourcePosition(Line, Column + 1);
        }

        public SourcePosition PreviousColumn()
        {
            if (Column == 0)
            {
                throw new InvalidOperationException();
            }

            return new SourcePosition(Line, Column - 1);
        }

        public SourcePosition StartOfNextLine()
        {
            return new SourcePosition(Line + 1, 0);
        }
    }
}
