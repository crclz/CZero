using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using System.Text;

namespace CZero.Lexical
{
    public class SourcePosition
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
            return new SourcePosition(Line + 1, 1);
        }
    }
}
