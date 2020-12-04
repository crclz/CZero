using Ardalis.GuardClauses;
using CZero.Syntactic.Ast.Expressions;
using CZero.Syntactic.Ast.Expressions.OperatorExpression;
using System;
using System.Collections.Generic;
using System.Text;

namespace CZero.Syntactic.Policies
{
    static class StrongFactorPolicy
    {
        public static void CheckSingleExpression(ExpressionAst expression)
        {
            Guard.Against.NotType<OperatorExpressionAst>(expression, nameof(expression));
        }
    }
}
