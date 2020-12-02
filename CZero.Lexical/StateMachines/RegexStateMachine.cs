using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace CZero.Lexical.StateMachines
{
    class RegexStateMachine : IStateMachine
    {
        public Regex WorkingRegex { get; }

        public RegexStateMachine(string pattern)
        {
            Guard.Against.Null(pattern, nameof(pattern));

            // Should start with pattern
            pattern = '^' + pattern;

            WorkingRegex = new Regex(pattern);
        }

        public bool Consume(SourceStringReader reader)
        {
            var match = WorkingRegex.Match(reader.SourceCode, reader.Cursor);
            if (match.Success)
            {
                reader.Advance(match.Length);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
