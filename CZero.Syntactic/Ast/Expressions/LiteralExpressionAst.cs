using CZero.Lexical.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace CZero.Syntactic.Ast.Expressions
{
    public class LiteralExpressionAst : ExpressionAst
    {
        public LiteralToken Literal { get; }

        public LiteralExpressionAst(LiteralToken literal)
        {
            Literal = literal ?? throw new ArgumentNullException(nameof(literal));
        }
    }
}
