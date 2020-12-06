using CZero.Lexical.Tokens;
using CZero.Syntactic.Ast.Expressions;
using CZero.Syntactic.Ast.Expressions.OperatorExpression;
using CZero.Syntactic.Ast.Statements;
using CZero.Syntactic.Ast.Statements.Declarative;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public bool TryBlockStatement(out BlockStatementAst blockStatement)
        {
            // block_stmt -> '{' stmt* '}'
            var oldCursor = _reader._cursor;

            if (!_reader.AdvanceIfCurrentIsOperator(out OperatorToken leftBrace, Operator.LeftBrace))
                goto fail;

            var statements = new List<StatementAst>();
            while (TryStatement(out StatementAst statement))
                statements.Add(statement);

            if (!_reader.AdvanceIfCurrentIsOperator(out OperatorToken rightBrace, Operator.RightBrace))
                goto fail;

            blockStatement = new BlockStatementAst(leftBrace, statements, rightBrace);
            return true;

        fail:
            blockStatement = null;
            return restoreCursor(oldCursor);
        }

        public bool TryIfStatement(out IfStatementAst ifStatement)
        {
            var oldCursor = _reader._cursor;
            // if_stmt -> 'if' expr block_stmt ('else' (block_stmt | if_stmt))?

            if (!_reader.AdvanceIfCurrentIsKeyword(out KeywordToken @if, Keyword.If))
                goto fail;
            if (!TryExpression(out ExpressionAst condition))
                goto fail;
            if (!TryBlockStatement(out BlockStatementAst blockStatement))
                goto fail;

            KeywordToken @else = null;
            BlockStatementAst followingBlock = null;
            IfStatementAst followingIf = null;

            var cursor2 = _reader._cursor;

            bool tryElseAndFollowing()
            {
                var oldCursor = _reader._cursor;

                if (!_reader.AdvanceIfCurrentIsKeyword(out @else, Keyword.Else))
                    return false;

                var followingOk = TryBlockStatement(out followingBlock);
                if (!followingOk)
                    followingOk = TryIfStatement(out followingIf);

                return followingOk;
            }

            var ok = tryElseAndFollowing();
            if (!ok)
                _reader.SetCursor(cursor2);

            // ok
            ifStatement = new IfStatementAst(@if, condition, blockStatement, @else, followingBlock, followingIf);
            return true;
        fail:
            ifStatement = null;
            return restoreCursor(oldCursor);

        }

        public bool TryWhileStatement(out WhileStatementAst whileStatement)
        {
            // while_stmt -> 'while' expr block_stmt
            var oldCursor = _reader._cursor;

            if (!_reader.AdvanceIfCurrentIsKeyword(out KeywordToken @while, Keyword.While))
                goto fail;
            if (!TryExpression(out ExpressionAst condition))
                goto fail;
            if (!TryBlockStatement(out BlockStatementAst whileBlock))
                goto fail;

            whileStatement = new WhileStatementAst(@while, condition, whileBlock);
            return true;
        fail:
            whileStatement = null;
            return restoreCursor(oldCursor);
        }

        public bool TryReturnStatement(out ReturnStatementAst returnStatement)
        {
            // return_stmt -> 'return' expr? ';'
            var oldCursor = _reader._cursor;

            if (!_reader.AdvanceIfCurrentIsKeyword(out KeywordToken @return, Keyword.Return))
                goto fail;

            TryExpression(out ExpressionAst returnExpression);

            if (!_reader.AdvanceIfCurrentIsOperator(out OperatorToken semicolon, Operator.Semicolon))
                goto fail;

            returnStatement = new ReturnStatementAst(@return, returnExpression, semicolon);
            return true;

        fail:
            returnStatement = null;
            return restoreCursor(oldCursor);
        }

        public bool TryEmptyStatement(out EmptyStatementAst emptyStatement)
        {
            // empty_stmt -> ';'
            var oldCursor = _reader._cursor;

            if (!_reader.AdvanceIfCurrentIsOperator(out OperatorToken semicolon, Operator.Semicolon))
                goto fail;

            emptyStatement = new EmptyStatementAst(semicolon);
            return true;

        fail:
            emptyStatement = null;
            return restoreCursor(oldCursor);
        }

        public bool TryBreakStatement(out BreakStatementAst breakStatement)
        {
            // break_stmt -> 'break' ';'
            var oldCursor = _reader._cursor;

            if (!_reader.AdvanceIfCurrentIsKeyword(out KeywordToken @break, Keyword.Break))
                goto fail;
            if (!_reader.AdvanceIfCurrentIsOperator(out OperatorToken @semicolon, Operator.Semicolon))
                goto fail;

            breakStatement = new BreakStatementAst(@break, semicolon);
            return true;

        fail:
            breakStatement = null;
            return restoreCursor(oldCursor);
        }

        public bool TryContinueStatement(out ContinueStatementAst continueStatement)
        {
            // continue_stmt -> 'continue' ';'
            var oldCursor = _reader._cursor;

            if (!_reader.AdvanceIfCurrentIsKeyword(out KeywordToken @continue, Keyword.Continue))
                goto fail;
            if (!_reader.AdvanceIfCurrentIsOperator(out OperatorToken @semicolon, Operator.Semicolon))
                goto fail;

            continueStatement = new ContinueStatementAst(@continue, semicolon);
            return true;

        fail:
            continueStatement = null;
            return restoreCursor(oldCursor);
        }

        public bool TryStatement(out StatementAst statement)
        {
            /*
             stmt ->
                  expr_stmt
                | decl_stmt
                | if_stmt
                | while_stmt
                | return_stmt
                | block_stmt
                | empty_stmt

                | break_stmt
                | continue_stmt
            */

            statement = null;

            if (TryExpressionStatement(out ExpressionStatementAst expressionStatement))
                statement = expressionStatement;
            else if (TryDeclarationStatement(out DeclarationStatementAst declarationStatement))
                statement = declarationStatement;
            else if (TryIfStatement(out IfStatementAst ifStatement))
                statement = ifStatement;
            else if (TryWhileStatement(out WhileStatementAst whileStatement))
                statement = whileStatement;
            else if (TryReturnStatement(out ReturnStatementAst returnStatement))
                statement = returnStatement;
            else if (TryBlockStatement(out BlockStatementAst blockStatement))
                statement = blockStatement;
            else if (TryEmptyStatement(out EmptyStatementAst emptyStatement))
                statement = emptyStatement;
            else if (TryContinueStatement(out ContinueStatementAst continueStatement))
                statement = continueStatement;
            else if (TryBreakStatement(out BreakStatementAst breakStatement))
                statement = breakStatement;

            return statement != null;
        }
    }
}
