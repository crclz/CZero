using Ardalis.GuardClauses;
using CZero.Lexical.Tokens;
using CZero.Syntactic.Ast.Expressions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CZero.Syntactic.Ast.Statements.Declarative
{
    public class LetDeclarationStatementAst : DeclarationStatementAst
    {
        // 'let' IDENT ':' ty ('=' expr)? ';'

        public KeywordToken Let { get; }
        public IdentifierToken Name { get; }
        public OperatorToken Colon { get; }
        public IdentifierToken Type { get; }

        /// <summary>
        /// Optional
        /// </summary>
        public OperatorToken Assign { get; }

        /// <summary>
        /// Optional
        /// </summary>
        public ExpressionAst InitialExpression { get; }

        public OperatorToken Semicolon { get; }

        public LetDeclarationStatementAst(
            KeywordToken let, IdentifierToken name, OperatorToken colon,
            IdentifierToken type, OperatorToken assign, ExpressionAst initialExpression,
            OperatorToken semicolon)
        {
            Let = let ?? throw new ArgumentNullException(nameof(let));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Colon = colon ?? throw new ArgumentNullException(nameof(colon));
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Assign = assign;
            InitialExpression = initialExpression;
            Semicolon = semicolon ?? throw new ArgumentNullException(nameof(semicolon));


            if (let.Keyword != Keyword.Let)
                throw new ArgumentException(nameof(let));

            if (colon.Value != Operator.Colon)
                throw new ArgumentException(nameof(colon));

            if (assign != null && assign.Value != Operator.Assign)
                throw new ArgumentException(nameof(assign));

            if (semicolon.Value != Operator.Semicolon)
                throw new ArgumentNullException(nameof(semicolon));
        }
    }
}
