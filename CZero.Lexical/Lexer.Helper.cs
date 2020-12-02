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
            var startPosition = _reader.Position;

            var (result, isDouble) = _reader.RegexMatch(RegexList.DoubleLiteral);
            if (isDouble)
            {
                if (!double.TryParse(result, out double val))
                    throw new LexerException("Double literal overflow");
                token = new DoubleLiteralToken(val, startPosition);
                return true;
            }

            token = null;
            return false;
        }

        private bool TryMatchUnsigned(out UInt64LiteralToken token)
        {
            var startPosition = _reader.Position;

            var (result, isUnsigned) = _reader.RegexMatch(RegexList.UnsignedLiteral);
            if (isUnsigned)
            {
                if (!ulong.TryParse(result, out ulong val))
                    throw new LexerException("Unsigned literal overflow");

                token = new UInt64LiteralToken(val, startPosition);
                return true;
            }

            token = null;
            return false;
        }

        private bool TryMatchStringLiteral(out StringLiteralToken token)
        {
            var startPosition = _reader.Position;

            var (result, isString) = _reader.RegexMatch(RegexList.StringLiteral);
            if (!isString)
            {
                token = null;
                return false;
            }

            // 语义约束
            if (!StringLiteralToken.SatisfyConstraints(result))
                throw new LexerException("String not satisfying literal contraints");

            // Replace escape chars
            string realValue = ReplaceEscapeChars(result);

            token = new StringLiteralToken(realValue, startPosition);
            return true;
        }

        private bool TryMatchCharLiteral(out CharLiteralToken token)
        {
            var startPosition = _reader.Position;

            // CharLiteralToken
            var (result, isChar) = _reader.RegexMatch(RegexList.CharLiteral);
            if (!isChar)
            {
                token = null;
                return false;
            }

            // Replace escape chars
            char resultChar = CharIteralToChar(result);

            token = new CharLiteralToken(resultChar, startPosition);
            return true;
        }
    }
}
