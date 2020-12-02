using CZero.Lexical.Tokens;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace CZero.Lexical.Test.Tokens
{
    public class KeyworkTokenTest
    {
        [Fact]
        void KeywordReferenceTest()
        {
            var values = Enum.GetValues(typeof(Keyword));

            Assert.Equal(values.Length, KeywordToken.KeywordReference.Count);

            foreach (var o in values)
            {
                var enumValue = (Keyword)o;
                var keyword = enumValue.ToString().ToLower();
                var actualValue = KeywordToken.KeywordReference[keyword];

                Assert.Equal(enumValue, actualValue);
            }
        }
    }
}
