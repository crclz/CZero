using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using System.Text;

namespace CZero.Lexical.StateMachines
{
    abstract class StateMachine : IStateMachine
    {
        /// <summary>
        /// Consume is a wrapper around DoConsume, making it safer.
        /// </summary>
        public (string, bool) Consume(SourceStringReader reader)
        {
            Guard.Against.Null(reader, nameof(reader));

            var previousCursor = reader.Cursor;

            var (result, success) = DoConsume(reader);

            if (success)
            {
                var expectedResultString = reader.SourceCode
                    .Substring(previousCursor, reader.Cursor - previousCursor);

                if (expectedResultString != result)
                    throw new Exception("Code logic error. String doesn't match");
            }
            else
            {
                if (previousCursor != reader.Cursor)
                    throw new Exception("Code logic error. Cursor should not be moved.");
            }

            return (result, success);
        }

        public abstract (string, bool) DoConsume(SourceStringReader reader);
    }
}
