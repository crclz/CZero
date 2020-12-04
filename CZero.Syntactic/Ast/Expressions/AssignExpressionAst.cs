using CZero.Lexical.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace CZero.Syntactic.Ast.Expressions
{
    public class AssignExpressionAst : ExpressionAst
    {
        public IdentifierToken Identifier { get; }
        public OperatorToken Assign { get; }
        public ExpressionAst Expression { get; }

        public AssignExpressionAst(IdentifierToken identifier, OperatorToken assign, ExpressionAst expression)
        {
            Identifier = identifier ?? throw new ArgumentNullException(nameof(identifier));
            Assign = assign ?? throw new ArgumentNullException(nameof(assign));
            Expression = expression ?? throw new ArgumentNullException(nameof(expression));

            if (assign.Value != Operator.Assign)
                throw new ArgumentException(nameof(assign));
        }

    }
}
