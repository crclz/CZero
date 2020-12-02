using System;
using System.Collections.Generic;
using System.Text;

namespace CZero.Lexical
{
    static class RegexList
    {
        public static string IdentifierOrKeyword => @"[_a-zA-Z][_a-zA-Z0-9]*";
        public static string DoubleLiteral => @"\d+\.\d+([eE][+-]?\d+)?";
        public static string UnsignedLiteral => @"\d+";
        public static string StringLiteral => @"""([^\n""\\]|\\[\\""'nrt])*""";
        public static string CharLiteral => @"'([^\n'\\]|\\[\\""'nrt])'";
        public static string Comment => @"//.*\n";
    }
}
