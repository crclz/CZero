using CZero.Lexical.Tokens;
using CZero.Syntactic.Ast.Expressions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CZero.Syntactic.Ast.Statements.Declarative
{
    public class ConstDeclarationStatement : DeclarationStatementAst
    {
        // const_decl_stmt -> 'const' IDENT ':' ty '=' expr ';'

        public KeywordToken Const { get; }
        public IdentifierToken Name { get; }
        public OperatorToken Colon { get; }
        public IdentifierToken Type { get; }
        public OperatorToken Assign { get; }
        public ExpressionAst ValueExpression { get; }
        public OperatorToken Semicolon { get; }

        public ConstDeclarationStatement(
            KeywordToken @const, IdentifierToken name, OperatorToken colon,
            IdentifierToken type, OperatorToken assign, ExpressionAst valueExpression,
            OperatorToken semicolon)
        {
            Const = @const ?? throw new ArgumentNullException(nameof(@const));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Colon = colon ?? throw new ArgumentNullException(nameof(colon));
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Assign = assign ?? throw new ArgumentNullException(nameof(assign));
            ValueExpression = valueExpression ?? throw new ArgumentNullException(nameof(valueExpression));
            Semicolon = semicolon ?? throw new ArgumentNullException(nameof(semicolon));

            if (@const.Keyword != Keyword.Const)
                throw new ArgumentException(nameof(@const));

            if (colon.Value != Operator.Colon)
                throw new ArgumentException(nameof(colon));

            if (assign != null && assign.Value != Operator.Assign)
                throw new ArgumentException(nameof(assign));

            if (semicolon.Value != Operator.Semicolon)
                throw new ArgumentException(nameof(semicolon));
        }
    }
}
