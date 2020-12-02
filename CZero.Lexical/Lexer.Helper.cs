using CZero.Lexical.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CZero.Lexical
{
    public partial class Lexer
    {
        public bool RegexMatch(string pattern, out string result)
        {
            pattern = '^' + pattern;
            var regex = new Regex(pattern);

            var match = regex.Match(_reader.SourceCode, _reader.Cursor);


            if (match.Success)
            {
                _reader.Advance(match.Length);

                result = match.Value;
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }

        private bool TryMatchOperator(out OperatorToken token)
        {
            var startPosition = _reader.Position;

            // Should match from long to short
            var operatorList = OperatorToken.OperatorReference.Keys
                .OrderByDescending(p => p.Length).ToList();

            foreach (var op in operatorList)
            {
                var pattern = RegexList.OperatorPatterns[op];
                Debug.Assert(pattern != null);

                if (RegexMatch(pattern, out string result))
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

            if (RegexMatch(RegexList.DoubleLiteral, out string result))
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

            if (RegexMatch(RegexList.UnsignedLiteral, out string result))
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

            if (!RegexMatch(RegexList.StringLiteral, out string result))
            {
                token = null;
                return false;
            }

            // Replace escape chars
            string realValue = ReplaceEscapeChars(result);

            token = new StringLiteralToken(realValue, startPosition);
            return true;
        }

        private bool TryMatchCharLiteral(out CharLiteralToken token)
        {
            var startPosition = _reader.Position;

            // CharLiteralToken
            if (!RegexMatch(RegexList.CharLiteral, out string result))
            {
                token = null;
                return false;
            }

            // Replace escape chars
            char resultChar = ReplaceEscapeChars(result).Single();

            token = new CharLiteralToken(resultChar, startPosition);
            return true;
        }

        private static string ReplaceEscapeChars(string s)
        {
            throw new NotImplementedException();
        }
    }
}
