using System;
using System.Collections.Generic;
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
    }
}
