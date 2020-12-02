using System;
using System.Collections.Generic;
using System.Text;

namespace CZero.Lexical.StateMachines
{
    interface IStateMachine
    {
        /// <summary>
        /// When fail, returns (null, false) and do not move the cursor. <br/>
        /// When success, returns (matched string, true) and move the cursor
        /// </summary>
        /// <param name="reader">The source code reader</param>
        public (string, bool) Consume(SourceStringReader reader);
    }
}
