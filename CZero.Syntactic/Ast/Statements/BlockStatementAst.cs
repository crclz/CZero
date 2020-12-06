using Ardalis.GuardClauses;
using CZero.Lexical.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CZero.Syntactic.Ast.Statements
{
    public class BlockStatementAst : StatementAst
    {
        // block_stmt -> '{' stmt* '}'

        public OperatorToken LeftBrace { get; }
        public IReadOnlyList<StatementAst> Statements { get; }
        public OperatorToken RightBrace { get; }

        public BlockStatementAst(
            OperatorToken leftBrace, IEnumerable<StatementAst> statements, OperatorToken rightBrace)
        {
            LeftBrace = leftBrace ?? throw new ArgumentNullException(nameof(leftBrace));
            RightBrace = rightBrace ?? throw new ArgumentNullException(nameof(rightBrace));

            Guard.Against.Null(statements, nameof(statements));
            Guard.Against.NullElement(statements, nameof(statements));
            Statements = statements.ToList();
        }
    }
}
