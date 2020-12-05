using CZero.Lexical.Tokens;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace CZero.Syntactic.Test
{
    public class TokenReaderTest
    {
        private TokenReader GetReader()
        {
            var tokens = new Token[]
            {
                new OperatorToken(Operator.Arrow, (0, 0)),
            };

            return new TokenReader(tokens);
        }

        [Fact]
        void Current_Normal_returns_token()
        {
            var a = new OperatorToken(Operator.Arrow, (0, 0));
            var reader = new TokenReader(new Token[] { a });

            Assert.Equal(a, reader.Current());
            Assert.Equal(a, reader.Current());
            Assert.Equal(a, reader.Current());
            Assert.Equal(a, reader.Current());
        }

        [Fact]
        void Current_AtEnd_throws()
        {
            var reader = new TokenReader(new Token[] { });

            Assert.Throws<InvalidOperationException>(() => reader.Current());
        }

        [Fact]
        void Advance_when_normal_adds_cursor()
        {
            var a = new OperatorToken(Operator.Arrow, (0, 0));
            var reader = new TokenReader(new Token[] { a });

            reader.Advance();
            Assert.Equal(1, reader._cursor);
        }

        [Fact]
        void Advance_at_end_throws_SyntacticException()
        {
            var reader = new TokenReader(new Token[] { });

            Assert.Throws<InvalidOperationException>(() => reader.Advance());
        }

        [Fact]
        void AdvanceIfCurrentIsType_advances_and_returns_token_when_ok()
        {
            var a = new OperatorToken(Operator.Arrow, (0, 0));
            var reader = new TokenReader(new Token[] { a });

            Assert.True(reader.AdvanceIfCurrentIsType(out OperatorToken t));
            Assert.Equal(a, t);
            Assert.Equal(1, reader._cursor);
        }

        [Fact]
        void AdvanceIfCurrentIsType_returns_false_and_not_advance_when_not_match_type()
        {
            var a = new OperatorToken(Operator.Arrow, (0, 0));
            var reader = new TokenReader(new Token[] { a });

            Assert.False(reader.AdvanceIfCurrentIsType(out KeywordToken t));
            Assert.Null(t);
            Assert.Equal(0, reader._cursor);
        }

        [Fact]
        void AdvanceIfCurrentIsType_return_false_when_reache_end()
        {
            var reader = new TokenReader(new Token[] { });
            Assert.False(reader.AdvanceIfCurrentIsType(out OperatorToken t));
            Assert.Null(t);
        }

    }
}
