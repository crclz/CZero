using CZero.Lexical.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace CZero.Syntactic.Ast.Expressions
{
    public class GroupExpressionAst : ExpressionAst
    {
        public OperatorToken LeftParen { get; }

        public ExpressionAst Expression { get; }

        public OperatorToken RightParen { get; }

        public GroupExpressionAst(OperatorToken leftParen, ExpressionAst expression, OperatorToken rightParen)
        {
            LeftParen = leftParen ?? throw new ArgumentNullException(nameof(leftParen));
            Expression = expression ?? throw new ArgumentNullException(nameof(expression));
            RightParen = rightParen ?? throw new ArgumentNullException(nameof(rightParen));

            if (leftParen.Value != Operator.LeftParen)
                throw new ArgumentException(nameof(leftParen));

            if (RightParen.Value != Operator.RightParen)
                throw new ArgumentException(nameof(rightParen));
        }
    }
}
