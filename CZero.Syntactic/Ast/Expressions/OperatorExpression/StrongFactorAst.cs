using Ardalis.GuardClauses;
using CZero.Syntactic.Policies;
using System;
using System.Collections.Generic;
using System.Text;

namespace CZero.Syntactic.Ast.Expressions.OperatorExpression
{
    public class StrongFactorAst
    {
        public ExpressionAst SingleExpression { get; }

        public StrongFactorAst(ExpressionAst singleExpression)
        {
            Guard.Against.Null(singleExpression, nameof(singleExpression));

            // Exclude operator_expr according to Grammer
            StrongFactorPolicy.CheckSingleExpression(singleExpression);

            SingleExpression = singleExpression;
        }
    }
}
