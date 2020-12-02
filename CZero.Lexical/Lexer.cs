using CZero.Lexical.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

// https://c0.karenia.cc/c0/token.html

namespace CZero.Lexical
{
    public partial class Lexer
    {
        private SourceStringReader _reader { get; set; }

        public Lexer(string sourceCode)
        {
            _reader = new SourceStringReader(sourceCode);
        }

        public IEnumerable<Token> Parse()
        {
            while (!_reader.ReachedEnd)
            {
                if (!_reader.AdvanceUntilNonWhite())
                    yield break;// End producing token

                var startPosition = _reader.Position;

                char c = _reader.CurrentChar();
                if (char.IsLetter(c) || c == '_')
                {
                    var (result, success) = _reader.RegexMatch(RegexList.IdentifierOrKeyword);
                    Debug.Assert(success);// Worst: IdentifierToken{name=CurrentChar}

                    // Determine token is keyword or identifier
                    if (KeywordToken.IsKeyword(result))
                    {
                        yield return KeywordToken.FromKeywordString(result, startPosition);
                    }
                    else
                    {
                        yield return new IdentifierToken(result, startPosition);
                    }
                }
                else if (char.IsDigit(c))
                {
                    // Double ----> Unsigned

                    // Double
                    if (TryMatchDouble(out DoubleLiteralToken doubleLiteralToken))
                    {
                        yield return doubleLiteralToken;
                        continue;
                    }

                    var success = TryMatchUnsigned(out UInt64LiteralToken uInt64LiteralToken);
                    Debug.Assert(success);// Worst: 1-digit unsigned iteral token

                    yield return uInt64LiteralToken;
                    continue;
                }
                else if (c == '"')
                {
                    if (!TryMatchStringLiteral(out StringLiteralToken stringLiteral))
                        throw new LexerException("Failed to parse String Literal");

                    yield return stringLiteral;
                    continue;
                }
                else if (c == '\'')
                {
                    if (!TryMatchCharLiteral(out CharLiteralToken charLiteral))
                        throw new LexerException("Failed to parse Char Literal");

                    yield return charLiteral;
                    continue;
                }
                else
                {
                    // Match operator
                    if (TryMatchOperator(out OperatorToken operatorToken))
                    {
                        yield return operatorToken;
                        continue;
                    }

                    // Match comment
                    var (comment, isComment) = _reader.RegexMatch(RegexList.Comment);
                    if (isComment)
                    {
                        // Ignore comment, do not produce token
                        continue;
                    }

                    // Match nothing, throw
                    throw new LexerException($"Unexpected character '{c}' at " +
                        $"({startPosition.Line},{startPosition.Column})");
                }
            }
        }


    }
}
