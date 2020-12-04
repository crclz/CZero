using Ardalis.GuardClauses;
using CZero.Lexical.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace CZero.Syntactic.Ast.Expressions
{
    public class IdentExpressionAst : ExpressionAst
    {
        public IdentifierToken IdentifierToken { get; }

        public IdentExpressionAst(IdentifierToken token)
        {
            Guard.Against.Null(token, nameof(token));

            IdentifierToken = token;
        }
    }
}
