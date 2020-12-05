using CZero.Lexical.Tokens;
using CZero.Syntactic.Ast.Expressions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CZero.Syntactic.Ast.Statements
{
    public class WhileStatementAst : StatementAst
    {
        // while_stmt -> 'while' expr block_stmt

        public KeywordToken While { get; }
        public ExpressionAst ConditionExpression { get; }
        public BlockStatementAst WhileBlock { get; }

        public WhileStatementAst(
            KeywordToken @while, ExpressionAst conditionExpression, BlockStatementAst whileBlock)
        {
            While = @while ?? throw new ArgumentNullException(nameof(@while));
            ConditionExpression = conditionExpression ?? throw new ArgumentNullException(nameof(conditionExpression));
            WhileBlock = whileBlock ?? throw new ArgumentNullException(nameof(whileBlock));

            if (@while.Keyword != Keyword.While)
                throw new ArgumentException();
        }
    }
}
