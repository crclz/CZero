using CZero.Lexical.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace CZero.Syntactic.Ast.Statements
{
    public class EmptyStatementAst : StatementAst
    {
        public OperatorToken Semicolon { get; }

        public EmptyStatementAst(OperatorToken semicolon)
        {
            Semicolon = semicolon ?? throw new ArgumentNullException(nameof(semicolon));

            if (semicolon.Value != Operator.Semicolon)
                throw new ArgumentException();
        }
    }
}
