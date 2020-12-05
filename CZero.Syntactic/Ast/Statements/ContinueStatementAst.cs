using CZero.Lexical.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace CZero.Syntactic.Ast.Statements
{
    public class ContinueStatementAst : StatementAst
    {
        public KeywordToken Continue { get; }
        public OperatorToken Semicolon { get; }

        public ContinueStatementAst(KeywordToken @continue, OperatorToken semicolon)
        {
            Continue = @continue ?? throw new ArgumentNullException(nameof(@continue));
            Semicolon = semicolon ?? throw new ArgumentNullException(nameof(semicolon));

            if (@continue.Keyword != Keyword.Continue)
                throw new ArgumentException();

            if (semicolon.Value != Operator.Semicolon)
                throw new ArgumentException();
        }
    }
}
