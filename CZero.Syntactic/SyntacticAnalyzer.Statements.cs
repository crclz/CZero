using CZero.Lexical.Tokens;
using CZero.Syntactic.Ast.Expressions;
using CZero.Syntactic.Ast.Expressions.OperatorExpression;
using CZero.Syntactic.Ast.Statements;
using CZero.Syntactic.Ast.Statements.Declarative;
using System;
using System.Collections.Generic;
using System.Text;

namespace CZero.Syntactic
{
    public partial class SyntacticAnalyzer
    {
        public bool TryExpressionStatement(out ExpressionStatementAst expressionStatement)
        {
            var oldCursor = _reader._cursor;

            // expr_stmt -> expr ';'

            if (!TryExpression(out ExpressionAst expression))
            {
                expressionStatement = null;
                return restoreCursor(oldCursor);
            }

            if (!_reader.AdvanceIfCurrentIsOperator(out OperatorToken semicolon, Operator.Semicolon))
            {
                expressionStatement = null;
                return restoreCursor(oldCursor);
            }

            expressionStatement = new ExpressionStatementAst(expression, semicolon);
            return true;
        }

        public bool TryDeclarationStatement(out DeclarationStatementAst declarationStatement)
        {
            var oldCursor = _reader._cursor;

            // decl_stmt -> let_decl_stmt | const_decl_stmt

            if (!TryConstDeclarationStatement(out ConstDeclarationStatement constDeclaration))
            {
                declarationStatement = constDeclaration;
                return true;
            }
            else if (!TryLetDeclarationStatement(out LetDeclarationStatementAst letDeclaration))
            {
                declarationStatement = letDeclaration;
                return true;
            }
            declarationStatement = null;
            return restoreCursor(oldCursor);
        }

        public bool TryConstDeclarationStatement(out ConstDeclarationStatement constDeclaration)
        {
            var oldCursor = _reader._cursor;

            // const_decl_stmt -> 'const' IDENT ':' ty '=' expr ';'

            // const
            if (!_reader.AdvanceIfCurrentIsKeyword(out KeywordToken @const, Keyword.Const))
            {
                constDeclaration = null;
                return restoreCursor(oldCursor);
            }

            // IDENT
            if (!_reader.AdvanceIfCurrentIsType(out IdentifierToken identifier))
            {
                constDeclaration = null;
                return restoreCursor(oldCursor);
            }

            // ':'
            if (!_reader.AdvanceIfCurrentIsOperator(out OperatorToken colon, Operator.Colon))
            {
                constDeclaration = null;
                return restoreCursor(oldCursor);
            }

            // ty (IDENT)
            if (!_reader.AdvanceIfCurrentIsType(out IdentifierToken type))
            {
                constDeclaration = null;
                return restoreCursor(oldCursor);
            }

            // '='
            if (!_reader.AdvanceIfCurrentIsOperator(out OperatorToken assign, Operator.Assign))
            {
                constDeclaration = null;
                return restoreCursor(oldCursor);
            }

            // expr
            if (!TryExpression(out ExpressionAst valueExpression))
            {
                constDeclaration = null;
                return restoreCursor(oldCursor);
            }

            // ';'
            if (!_reader.AdvanceIfCurrentIsOperator(out OperatorToken semicolon, Operator.Semicolon))
            {
                constDeclaration = null;
                return restoreCursor(oldCursor);
            }

            constDeclaration = new ConstDeclarationStatement(
                @const, identifier, colon,
                type, assign, valueExpression, semicolon);
            return true;
        }

        public bool TryLetDeclarationStatement(out LetDeclarationStatementAst letDeclaration)
        {
            var oldCursor = _reader._cursor;

            // 'let' IDENT ':' ty ('=' expr)? ';'

            // 'let'
            if (!_reader.AdvanceIfCurrentIsKeyword(out KeywordToken let, Keyword.Let))
            {
                letDeclaration = null;
                return restoreCursor(oldCursor);
            }

            // IDENT
            if (!_reader.AdvanceIfCurrentIsType(out IdentifierToken identifier))
            {
                letDeclaration = null;
                return restoreCursor(oldCursor);
            }

            // ':'
            if (!_reader.AdvanceIfCurrentIsOperator(out OperatorToken colon, Operator.Colon))
            {
                letDeclaration = null;
                return restoreCursor(oldCursor);
            }

            // ty (IDENT)
            if (!_reader.AdvanceIfCurrentIsType(out IdentifierToken type))
            {
                letDeclaration = null;
                return restoreCursor(oldCursor);
            }


            // ('=' expr)?

            OperatorToken assign = null;
            ExpressionAst valueExpression = null;

            bool tryAssignlExpr()
            {
                var oldCursor = _reader._cursor;
                if (!_reader.AdvanceIfCurrentIsOperator(out assign, Operator.Assign))
                    return restoreCursor(oldCursor);
                if (!TryExpression(out valueExpression))
                    return restoreCursor(oldCursor);
                return true;
            }

            if (!tryAssignlExpr())
            {
                // Ensure these shits are null
                assign = null;
                valueExpression = null;
            }


            // ';'
            if (!_reader.AdvanceIfCurrentIsOperator(out OperatorToken semicolon, Operator.Semicolon))
            {
                letDeclaration = null;
                return restoreCursor(oldCursor);
            }

            // ok
            letDeclaration = new LetDeclarationStatementAst(
                let, identifier, colon, type, assign, valueExpression, semicolon);
            return true;
        }
    }
}
