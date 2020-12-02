using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using System.Text;

namespace CZero.Lexical.Tokens
{
    public class OperatorToken : Token
    {
        public Operator Value { get; }

        public OperatorToken(Operator op, SourcePosition position) : base(position)
        {
            Guard.Against.OutOfRange(op, nameof(op));
            Value = op;
        }

        public static OperatorToken FromString(string opString, SourcePosition position)
        {
            Guard.Against.Null(opString, nameof(opString));
            Guard.Against.Null(position, nameof(position));

            if (!OperatorReference.ContainsKey(opString))
                throw new AggregateException(nameof(opString));

            var op = OperatorReference[opString];
            return new OperatorToken(op, position);
        }

        // Mapping examined
        public static IReadOnlyDictionary<string, Operator> OperatorReference
            = new Dictionary<string, Operator>
            {
                {"+", Operator.Plus },
                {"-", Operator.Minus },
                {"*", Operator.Mult },
                {"/", Operator.Divide },
                {"=", Operator.Assign },
                {"==", Operator.Equal },
                {"!=", Operator.NotEqual },
                {"<", Operator.LessThan },
                {">", Operator.GreaterThan },
                {"<=", Operator.LessEqual },
                {">=", Operator.GreaterEqual },
                {"(", Operator.LeftParen },
                {")", Operator.RightParen },
                {"{", Operator.LeftBrace },
                {"}", Operator.RightBrace },
                {"->", Operator.Arrow },
                {",", Operator.Comma },
                {":", Operator.Colon },
                {";", Operator.Semicolon }
            };
    }

    public enum Operator
    {
        Plus,
        Minus,
        Mult,
        Divide,
        Assign,
        Equal,
        NotEqual,

        /// <summary>
        /// <
        /// </summary>
        LessThan,

        /// <summary>
        /// >
        /// </summary>
        GreaterThan,

        /// <summary>
        /// <=
        /// </summary>
        LessEqual,

        /// <summary>
        /// >=
        /// </summary>
        GreaterEqual,

        /// <summary>
        /// (
        /// </summary>
        LeftParen,

        /// <summary>
        /// )
        /// </summary>
        RightParen,

        /// <summary>
        /// {
        /// </summary>
        LeftBrace,

        /// <summary>
        /// }
        /// </summary>
        RightBrace,

        /// <summary>
        /// ->
        /// </summary>
        Arrow,

        /// <summary>
        /// ,
        /// </summary>
        Comma,

        /// <summary>
        /// :
        /// </summary>
        Colon,

        /// <summary>
        /// ;
        /// </summary>
        Semicolon,
    }
}
