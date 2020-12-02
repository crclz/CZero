using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using System.Text;

namespace CZero.Lexical.StateMachines
{
    interface IStateMachine
    {
        /// <summary>
        /// When fail, returns false and do not move the cursor. <br/>
        /// When success, returns true and move the cursor.
        /// </summary>
        /// <param name="reader">The source code reader</param>
        public bool Consume(SourceStringReader reader);

        /// <summary>
        /// An wrapper around method Comsume. Making it easier and safer to use.
        /// </summary>
        public (string, bool) HelperConsume(SourceStringReader reader)
        {
            Guard.Against.Null(reader, nameof(reader));

            var oldCoursor = reader.Cursor;

            var success = Consume(reader);

            if (success)
            {
                var result = reader.SourceCode.Substring(oldCoursor, reader.Cursor);
                return (result, false);
            }
            else
            {
                if (oldCoursor != reader.Cursor)
                    throw new Exception("Code logic error. Cursor should not be moved.");

                return (null, false);
            }
        }
    }
}
