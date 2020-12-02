using CZero.Lexical.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace CZero.Lexical
{
    public partial class Lexer
    {
        private bool TryMatchOperator(out OperatorToken token)
        {
            var startPosition = _reader.Position;

            // Should match from long to short
            var operatorList = OperatorToken.OperatorReference.Keys
                .OrderByDescending(p => p.Length).ToList();

            foreach (var op in operatorList)
            {
                if (StringSliceStartsWith(_reader.SourceCode, _reader.Cursor, op))
                {
                    // Advance the cursor
                    _reader.Advance(op.Length);

                    token = OperatorToken.FromString(op, startPosition);
                    return true;
                }
            }

            token = null;
            return false;
        }

        private bool TryMatchDouble(out DoubleLiteralToken token)
        {

        }
    }
}
