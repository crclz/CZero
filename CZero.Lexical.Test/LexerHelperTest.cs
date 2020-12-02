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
    }
}
