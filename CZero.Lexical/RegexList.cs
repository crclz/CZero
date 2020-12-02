using CZero.Lexical.Tokens;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace CZero.Lexical
{
    static class RegexList
    {
        public static string IdentifierOrKeyword => @"[_a-zA-Z][_a-zA-Z0-9]*";
        public static string DoubleLiteral => @"\d+\.\d+([eE][+-]?\d+)?";
        public static string UnsignedLiteral => @"\d+";
        public static string StringLiteral => @"""([^\r\n\t""\\]|\\[\\""'nrt])*""";
        public static string CharLiteral => @"'([^\r\n\t'\\]|\\[\\""'nrt])'";
        public static string Comment => @"//.*\n";

        internal static IReadOnlyDictionary<string, string> OperatorPatterns { get; }
            = GenerateOperatorPatterns();


        // TODO: test this shit
        private static Dictionary<string, string> GenerateOperatorPatterns()
        {
            var patterns = new Dictionary<string, string>();

            foreach (var op in OperatorToken.OperatorReference.Keys)
            {
                var builder = new StringBuilder();

                var pattern = Regex.Escape(op);

                patterns.Add(op, pattern);
            }

            return patterns;
        }
    }
}
