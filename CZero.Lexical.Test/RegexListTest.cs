using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Xunit;

namespace CZero.Lexical.Test
{
    public class RegexListTest
    {
        static bool FullMatch(string pattern, string text)
        {
            pattern = "^" + pattern + "$";
            return Regex.IsMatch(text, pattern);
        }

        [Fact]
        void IdentifierOrKeyword_MatchTest()
        {
            var samples = new string[]
            {
                "a","_","ab","A","_s","if",
                "s_1","s1","s__"
            };
            foreach (var text in samples)
            {
                Assert.True(FullMatch(RegexList.IdentifierOrKeyword, text));
            }
        }

        [Fact]
        void IdentifierOrKeyword_NotMatchTest()
        {
            var samples = new string[]
            {
                "1a","1","1_","s_ 1","as d"
            };
            foreach (var text in samples)
            {
                Assert.False(FullMatch(RegexList.IdentifierOrKeyword, text));
            }
        }

        [Fact]
        void DoubleLiteral_MatchTest()
        {
            var samples = new string[]
            {
                "1.23","1.0","1.0e5","1.0E-5","1.0e+5"
            };
            foreach (var text in samples)
            {
                Assert.True(FullMatch(RegexList.DoubleLiteral, text));
            }
        }

        [Fact]
        void DoubleLiteral_NotMatchTest()
        {
            var samples = new string[]
            {
                ".23","1.","1.0e5.0","1a0","1.0e+"
            };
            foreach (var text in samples)
            {
                Assert.False(FullMatch(RegexList.DoubleLiteral, text));
            }
        }

        [Fact]
        void StringLiteral_MatchTest()
        {
            var samples = new string[]
            {
                @"""""",@""" """,@""" """,@"""abacaF""",
                "\"r\"","\"t\"","\"n\"",@"""\\"""
            };
            foreach (var text in samples)
            {
                Assert.True(FullMatch(RegexList.StringLiteral, text));
            }
        }

        [Fact]
        void StringLiteral_NotMatchTest()
        {
            var samples = new string[]
            {
                @"""a",@"""a",@"""\c""",
                "\"a\n\""
            };
            foreach (var text in samples)
            {
                Assert.False(FullMatch(RegexList.StringLiteral, text));
            }
        }

        [Fact]
        void CharLiteral_MatchTest()
        {
            var samples = new string[]
            {
                "'a'","' '",@"'\n'",@"'\r'",@"'\t'",@"'\\'",
            };
            foreach (var text in samples)
            {
                Assert.True(FullMatch(RegexList.CharLiteral, text));
            }
        }

        [Fact]
        void CharLiteral_NotMatchTest()
        {
            var samples = new string[]
            {
                @"'c",@"' ",@"'\q'",
                "'c\n'"
            };
            foreach (var text in samples)
            {
                Assert.False(FullMatch(RegexList.CharLiteral, text));
            }
        }
    }
}
