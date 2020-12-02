using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
        public static IReadOnlyDictionary<string, Operator> OperatorReference = GenerateReference();

        internal static Dictionary<string, Operator> GenerateReference()
        {
            var reference = new Dictionary<string, Operator>();

            var enumType = typeof(Operator);

            foreach (var i in Enum.GetValues(typeof(Operator)))
            {
                var enumValue = (Operator)i;

                var memberInfos = enumType.GetMember(enumValue.ToString());
                var enumValueMemberInfo = memberInfos.FirstOrDefault(m => m.DeclaringType == enumType);
                var valueAttributes =
                      enumValueMemberInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
                var description = ((DescriptionAttribute)valueAttributes[0]).Description;

                reference.Add(description, enumValue);
            }

            return reference;
        }
    }

    public enum Operator
    {
        [Description("+")]
        Plus,
        [Description("-")]
        Minus,
        [Description("*")]
        Mult,
        [Description("/")]
        Divide,
        [Description("=")]
        Assign,
        [Description("==")]
        Equal,
        [Description("!=")]
        NotEqual,
        [Description("<")]
        LessThan,
        [Description(">")]
        GreaterThan,
        [Description("<=")]
        LessEqual,
        [Description(">=")]
        GreaterEqual,
        [Description("(")]
        LeftParen,
        [Description(")")]
        RightParen,
        [Description("{")]
        LeftBrace,
        [Description("}")]
        RightBrace,
        [Description("->")]
        Arrow,
        [Description(",")]
        Comma,
        [Description(":")]
        Colon,
        [Description(";")]
        Semicolon,
    }
}
