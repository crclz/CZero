using Ardalis.GuardClauses;
using CZero.Lexical.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace CZero.Syntactic.Ast.Expressions
{
    public class NegateExpressionAst : ExpressionAst
    {
        public OperatorToken Minus { get; }
        public ExpressionAst Expression { get; }

        public NegateExpressionAst(OperatorToken minus, ExpressionAst expression)
        {
            Guard.Against.Null(minus, nameof(minus));
            Guard.Against.Null(expression, nameof(expression));

            if (minus.Value != Operator.Minus)
                throw new ArgumentException();

            Minus = minus;
            Expression = expression;
        }
    }
}
