using Ardalis.GuardClauses;
using CZero.Lexical.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("CZero.Syntactic.Test")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace CZero.Syntactic
{
    public class TokenReader
    {
        private Token[] _tokens { get; }
        internal int _cursor { get; private set; } = 0;

        public bool ReachedEnd => !(_cursor < _tokens.Length);

        public TokenReader(IEnumerable<Token> tokens)
        {
            Guard.Against.Null(tokens, nameof(tokens));

            _tokens = tokens.ToArray();
        }

        public Token Current()
        {
            if (ReachedEnd)
                throw new InvalidOperationException("Has reached end, cannot see current.");

            return _tokens[_cursor];
        }

        public void Advance()
        {
            if (ReachedEnd)
                throw new InvalidOperationException();

            _cursor++;
        }

        public void SetCursor(int position)
        {
            Guard.Against.OutOfRange(position, nameof(position), 0, _tokens.Length);

            _cursor = position;
        }

        public bool CurrentIsType<T>(out T token) where T : Token
        {
            if (ReachedEnd)
            {
                token = null;
                return false;
            }

            var t = Current();
            if (t is T t1)
            {
                token = t1;
                return true;
            }

            token = null;
            return false;
        }

        public bool AdvanceIfCurrentIsType<T>(out T token) where T : Token
        {
            if (!CurrentIsType(out T t))
            {
                token = null;
                return false;
            }

            Advance();
            token = t;
            return true;
        }

        public bool AdvanceIfCurrentIsOperator(out OperatorToken op, Operator kind)
        {
            if (!CurrentIsType(out OperatorToken token))
            {
                op = null; return false;
            }

            if (token.Value != kind)
            {
                op = null; return false;
            }

            Advance();

            op = token;
            return true;
        }

        public bool AdvanceIfCurrentIsKeyword(out KeywordToken keywordToken, Keyword keyword)
        {
            if (!CurrentIsType(out KeywordToken token))
            {
                keywordToken = null; return false;
            }

            if (token.Keyword != keyword)
            {
                keywordToken = null; return false;
            }

            Advance();

            keywordToken = token;
            return true;
        }
    }
}
