using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace CZero.Lexical.Tokens
{
    public class IdentifierToken : Token
    {
        public static string IdentifierPattern => "[_a-zA-Z][_a-zA-Z0-9]*";

        public string Value { get; }

        public IdentifierToken(string name, SourcePosition position) : base(position)
        {
            Guard.Against.NullOrWhiteSpace(name, nameof(name));

            if (!Regex.IsMatch(name, IdentifierPattern))
            {
                throw new ArgumentException($"Not match identifier pattern: {name}", nameof(name));
            }

            Value = name;
        }
    }
}
