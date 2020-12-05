using CZero.Lexical.Tokens;
using CZero.Syntactic.Ast.Expressions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CZero.Syntactic.Ast.Statements
{
    public class ExpressionStatementAst : StatementAst
    {
        public ExpressionAst Expression { get; }
        public OperatorToken Semicolon { get; }

        public ExpressionStatementAst(ExpressionAst expression, OperatorToken semicolon)
        {
            Expression = expression ?? throw new ArgumentNullException(nameof(expression));
            Semicolon = semicolon ?? throw new ArgumentNullException(nameof(semicolon));

            if (Semicolon.Value != Operator.Semicolon)
                throw new ArgumentException(nameof(semicolon));
        }
    }
}
