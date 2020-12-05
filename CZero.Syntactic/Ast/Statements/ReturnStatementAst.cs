using CZero.Lexical.Tokens;
using CZero.Syntactic.Ast.Expressions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CZero.Syntactic.Ast.Statements
{
    public class ReturnStatementAst : StatementAst
    {
        // return_stmt -> 'return' expr? ';'

        public KeywordToken Return { get; }

        /// <summary>
        /// Optional
        /// </summary>
        public ExpressionAst ReturnExpression { get; }

        public OperatorToken Semicolon { get; }

        public ReturnStatementAst(
            KeywordToken @return, ExpressionAst returnExpression, OperatorToken semicolon)
        {
            Return = @return ?? throw new ArgumentNullException(nameof(@return));
            ReturnExpression = returnExpression;
            Semicolon = semicolon ?? throw new ArgumentNullException(nameof(semicolon));

            if (@return.Keyword != Keyword.Return)
                throw new ArgumentException();

            if (semicolon.Value != Operator.Semicolon)
                throw new ArgumentException();
        }
    }
}
