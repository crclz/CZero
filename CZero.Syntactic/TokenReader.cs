using Ardalis.GuardClauses;
using CZero.Lexical.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("CZero.Syntactic.Test")]
namespace CZero.Syntactic
{
    class TokenReader
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

        public bool AdvanceIfCurrentIsType<T>(out T token) where T : Token
        {
            if (ReachedEnd)
                throw new SyntacticException("Unexpectedly reached end because of bad source code (token list");

            var t = Current();
            if (t is T t1)
            {
                token = t1;
                Advance();
                return true;
            }

            token = null;
            return false;
        }

        public T ExpectCurrentAndAdvance<T>() where T : Token
        {
            if (AdvanceIfCurrentIsType<T>(out T token))
            {
                return token;
            }

            throw new SyntacticException($"Expect: {typeof(T)}. Actual: {token.GetType()}");
        }
    }
}
