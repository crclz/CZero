using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using System.Text;

namespace CZero.Lexical.Tokens
{
    public class KeywordToken : Token
    {
        public Keyword Keyword { get; }

        public KeywordToken(Keyword keyword, SourcePosition position) : base(position)
        {
            // Enum is defined
            Guard.Against.OutOfRange(keyword, nameof(keyword));

            Keyword = keyword;
        }

        public static KeywordToken FromKeywordString(string keywordString, SourcePosition position)
        {
            Guard.Against.Null(keywordString, nameof(keywordString));

            if (!KeywordReference.TryGetValue(keywordString, out Keyword keyword))
            {
                throw new ArgumentException($"Is not keyword: {keywordString}", nameof(keywordString));
            }

            return new KeywordToken(keyword, position);
        }

        // Mapping examined
        public static IReadOnlyDictionary<string, Keyword> KeywordReference
            = new Dictionary<string, Keyword>
            {
                {"fn", Keyword.Fn },
                {"let", Keyword.Let },
                {"const", Keyword.Const },
                {"as", Keyword.As },
                {"while",Keyword.While },
                {"if", Keyword.If },
                {"else", Keyword.Else },
                {"return", Keyword.Return },
            };

        public static bool IsKeyword(string word)
        {
            Guard.Against.Null(word, nameof(word));

            return KeywordReference.ContainsKey(word);
        }
    }

    public enum Keyword
    {
        Fn,
        Let,
        Const,
        As,
        While,
        If,
        Else,
        Return,

        Break,
        Continue,
    }
}
