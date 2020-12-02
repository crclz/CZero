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
                    var (result, isDouble) = _reader.RegexMatch(RegexList.DoubleLiteral);
                    if (isDouble)
                    {
                        if (!double.TryParse(result, out double val))
                            throw new LexerException("Double literal overflow");
                        yield return new DoubleLiteralToken(val, startPosition);
                    }
                    else
                    {
                        bool isUnsigned;
                        (result, isUnsigned) = _reader.RegexMatch(RegexList.UnsignedLiteral);
                        Debug.Assert(isUnsigned);// Worst: 1-digit

                        if (!ulong.TryParse(result, out ulong val))
                            throw new LexerException("Unsigned literal overflow");
                        yield return new UInt64LiteralToken(val, startPosition);
                    }
                }
                else if (c == '"')
                {
                    // StringLiteralToken
                    var (result, isString) = _reader.RegexMatch(RegexList.StringLiteral);
                    if (!isString)
                        throw new LexerException();
                    // Replace escape chars
                    string realValue = ReplaceEscapeChars(result);

                    // TODO: 语义约束
                    if (!StringLiteralToken.SatisfyConstraints(realValue))
                        throw new LexerException("String not satisfying literal contraints");

                    yield return new StringLiteralToken(realValue, startPosition);
                }
                else if (c == '\'')
                {
                    // CharLiteralToken
                    var (result, isChar) = _reader.RegexMatch(RegexList.CharLiteral);
                    if (!isChar)
                        throw new LexerException();
                    // Replace escape chars
                    char resultChar = CharIteralToChar(result);
                    yield return new CharLiteralToken(resultChar, startPosition);
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
                        // Ignore comment
                        continue;
                    }


                    // Match nothing, throw
                    throw new LexerException();
                }
            }
        }

       
    }
}
