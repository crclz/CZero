using CZero.Lexical.Tokens;
using CZero.Syntactic.Ast.Expressions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CZero.Syntactic.Ast.Statements
{
    public class IfStatementAst : StatementAst
    {
        // if_stmt -> 'if' expr block_stmt ('else' (block_stmt | if_stmt))?

        public KeywordToken If { get; }
        public ExpressionAst ConditionExpression { get; }
        public BlockStatementAst BlockStatement { get; }

        /// <summary>
        /// Optional
        /// </summary>
        public KeywordToken Else { get; }

        /// <summary>
        /// Optional
        /// </summary>
        public BlockStatementAst FollowingBlock { get; }

        /// <summary>
        /// Optional
        /// </summary>
        public IfStatementAst FollowingIf { get; }

        public bool HasElseAndFollowing => Else != null;

        public IfStatementAst(
            KeywordToken @if, ExpressionAst conditionExpression, BlockStatementAst blockStatement,
            KeywordToken @else, BlockStatementAst followingBlock, IfStatementAst followingIf)
        {
            If = @if ?? throw new ArgumentNullException(nameof(@if));
            ConditionExpression = conditionExpression ?? throw new ArgumentNullException(nameof(conditionExpression));
            BlockStatement = blockStatement ?? throw new ArgumentNullException(nameof(blockStatement));

            Else = @else;
            FollowingBlock = followingBlock;
            FollowingIf = followingIf;

            if (@else == null)
            {
                if (!(followingBlock == null && followingIf == null))
                    throw new ArgumentException();
            }
            else
            {
                bool oneIsNotNull = followingBlock != null ^ FollowingIf != null;
                if (!oneIsNotNull)
                    throw new ArgumentException();
            }
        }
    }
}
