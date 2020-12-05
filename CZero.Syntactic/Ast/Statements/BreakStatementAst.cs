using CZero.Lexical.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace CZero.Syntactic.Ast.Statements
{
    public class BreakStatementAst : StatementAst
    {
        public KeywordToken Break { get; }
        public OperatorToken Semicolon { get; }

        public BreakStatementAst(KeywordToken @break, OperatorToken semicolon)
        {
            Break = @break ?? throw new ArgumentNullException(nameof(@break));
            Semicolon = semicolon ?? throw new ArgumentNullException(nameof(semicolon));

            if(@break.Keyword!= Keyword.Break)
                throw new ArgumentException();

            if (semicolon.Value != Operator.Semicolon)
                throw new ArgumentException();
        }
    }
}
