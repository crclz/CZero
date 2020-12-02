using CZero.Lexical.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Xunit;

namespace CZero.Lexical.Test
{
    public class LexerHelperTest
    {
        [Fact]
        void ReplaceEscapeCharsTest()
        {
            // \'、\"、\\、\n、\t、\r
            var s = @"\""\'\\\n\t\r \""c\'b\\6\n8\t1\r000";
            var realString = "\"\'\\\n\t\r \"c\'b\\6\n8\t1\r000";

            Assert.Equal(realString, Lexer.ReplaceEscapeChars(s));
        }

        [Fact]
        void RegexMatch_returns_the_matching_string_and_advances_reader_when_success()
        {
            var lexer = new Lexer("a1A.Hello");
            Assert.True(lexer.RegexMatch("[a-z_A-Z0-9]+", out string result));
            Assert.Equal("a1A", result);
            Assert.Equal(3, lexer.ReaderCursor);
        }

        [Fact]
        void RegexMatch_returns_false_and_do_not_move_cursor_when_fail()
        {
            var lexer = new Lexer("...a1A.Hello");
            Assert.False(lexer.RegexMatch("[a-z_A-Z0-9]+", out string result));
            Assert.Null(result);
            Assert.Equal(0, lexer.ReaderCursor);
        }

        [Fact]
        void TryMatchOperator_MatchTest_single()
        {
            var operators = OperatorToken.OperatorReference.Keys.ToList();

            foreach (var op in operators)
            {
                var lexer = new Lexer(op);
                Assert.True(lexer.TryMatchOperator(out OperatorToken opToken));
                Assert.Equal(OperatorToken.OperatorReference[op], opToken.Value);
            }
        }

        [Fact]
        void TryMatchOperator_MatchTest_follow_by_something()
        {
            var operators = OperatorToken.OperatorReference.Keys.ToList();

            foreach (var op in operators)
            {
                var lexer = new Lexer(op + "!!");
                Assert.True(lexer.TryMatchOperator(out OperatorToken opToken));
                Assert.Equal(OperatorToken.OperatorReference[op], opToken.Value);
            }
        }

        [Fact]
        void TryMatchOperator_NotMatchTest()
        {
            var operators = OperatorToken.OperatorReference.Keys.ToList();

            foreach (var op in operators)
            {
                var text = op;
                if (text.Length == 1)
                    text = "%";
                else
                    text = text.Substring(0, text.Length - 1);

                var lexer = new Lexer(text);
                var success = lexer.TryMatchOperator(out OperatorToken opToken);
                if (success)
                {
                    Assert.NotNull(opToken);
                    Assert.NotEqual(OperatorToken.OperatorReference[op], opToken.Value);
                }
                else
                {
                    Assert.Null(opToken);
                }
            }
        }

        [Fact]
        void TryMatchDouble_MatchTest()
        {
            var samples = new List<(string, double)>
            {
                ("1.0",1.0),
                ("0.12",0.12),
                ("1.11e3",1.11e3),
                ("1023.23e+3",1023.23e+3),
                ("10086.0012E-3",10086.0012E-3)
            };

            foreach (var (s, val) in samples)
            {
                var lexer = new Lexer(s);
                var success = lexer.TryMatchDouble(out DoubleLiteralToken doubleToken);
                Assert.True(success);
                Assert.Equal(val, doubleToken.Value);
                Assert.True(lexer.ReaderReachedEnd);
            }
        }

        [Fact]
        void TryMatchDouble_NotMatchTest()
        {
            var samples = new string[]
            {
                ".1","1a5","1a5e1","e101.11e1"
            };

            foreach (var s in samples)
            {
                var lexer = new Lexer(s);
                Assert.False(lexer.TryMatchOperator(out OperatorToken _));
            }
        }

        [Fact]
        void TryMatchUnsigned_MatchTest()
        {
            var samples = new (string, ulong)[]
            {
                ("12345678901234",12345678901234),
                ("00000123",00000123),
                ("0",0),
                ("1",1),
            };

            foreach (var (s, val) in samples)
            {
                var lexer = new Lexer(s);

                var success = lexer.TryMatchUnsigned(out UInt64LiteralToken ulongToken);
                Assert.True(success);
                Assert.Equal(val, ulongToken.Value);
                Assert.True(lexer.ReaderReachedEnd);
            }
        }

        [Fact]
        void TryMatchUnsigned_NotMatchTest()
        {
            var samples = new string[]
            {
                "-12345678901234",
                "x00000123",
                "",
                "d"
            };

            foreach (var s in samples)
            {
                var lexer = new Lexer(s);

                Assert.False(lexer.TryMatchUnsigned(out UInt64LiteralToken ulongToken));
                Assert.Null(ulongToken);
                Assert.Equal(0, lexer.ReaderCursor);
            }
        }

        [Fact]
        void TryMatchStringLiteral_MatchTest()
        {
            var realString = " \\hellio, world\n !";
            var lexer = new Lexer(@""" \\hellio, world\n !""");
            var success = lexer.TryMatchStringLiteral(out StringLiteralToken stringToken);
            Assert.True(success);
            Assert.Equal(realString, stringToken.Value);
        }

        [Fact]
        void TryMatchStringLiteral_NotMatchTest()
        {
            var lexer = new Lexer(@"""asdas");
            var success = lexer.TryMatchStringLiteral(out StringLiteralToken stringToken);

            Assert.False(success);
            Assert.Null(stringToken);
        }

        [Fact]
        void RemoveQuotes_throws_when_not_start_with_quote()
        {
            Assert.Throws<ArgumentException>(() => Lexer.RemoveQuotes("&adasd'"));
        }

        [Fact]
        void RemoveQuotes_throws_when_not_end_with_matching_quote()
        {
            Assert.Throws<ArgumentException>(() => Lexer.RemoveQuotes("'adasd\""));
            Assert.Throws<ArgumentException>(() => Lexer.RemoveQuotes("'adasd\""));
            Assert.Throws<ArgumentException>(() => Lexer.RemoveQuotes("\"adasd'"));
            Assert.Throws<ArgumentException>(() => Lexer.RemoveQuotes("\"adasd*"));
        }

        [Fact]
        void RemoveQuotes_removes_quotes_when_ok()
        {
            Assert.Equal("abcd", Lexer.RemoveQuotes(@"""abcd"""));
            Assert.Equal("abcd", Lexer.RemoveQuotes(@"'abcd'"));
        }

        [Fact]
        void TryMatchCharLiteral_MatchTest()
        {
            var lexer = new Lexer(@"'\r'");
            var success = lexer.TryMatchCharLiteral(out CharLiteralToken charToken);

            Assert.True(success);
            Assert.Equal('\r', charToken.Value);
        }

        [Fact]
        void TryMatchCharLiteral_NotMatchTest()
        {
            var lexer = new Lexer(@"'\'");
            var success = lexer.TryMatchCharLiteral(out CharLiteralToken charToken);

            Assert.False(success);
            Assert.Null(charToken);
        }
    }
}
