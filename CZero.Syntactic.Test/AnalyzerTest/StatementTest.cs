using CZero.Lexical.Tokens;
using CZero.Syntactic.Ast.Statements;
using CZero.Syntactic.Ast.Statements.Declarative;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace CZero.Syntactic.Test.AnalyzerTest
{
    public class StatementTest
    {
        [Fact]
        void ExpressionStatement_return_correct_ast_and_advance_cursor_when_success()
        {
            // Arrange
            var tokens = new Token[]
            {
                new DoubleLiteralToken(1.234,(0,0)),
                new OperatorToken( Operator.Semicolon, (0,0))
            };
            var reader = new TokenReader(tokens);
            var analyzer = new SyntacticAnalyzer(reader);

            // Act
            var success = analyzer.TryExpressionStatement(out ExpressionStatementAst expressionStatement);

            // Assert
            Assert.True(success);
            Assert.NotNull(expressionStatement);
            Assert.Equal(tokens[1], expressionStatement.Semicolon);
            Assert.Equal(1.234, expressionStatement.Expression.Calculate());// easier for checking

            Assert.True(reader.ReachedEnd);
        }

        [Fact]
        void ExpressionStatement_return_false_and_not_advance_when_fail()
        {
            // Arrange
            var tokens = new Token[]
            {
                new DoubleLiteralToken(1.234,(0,0)),
                new OperatorToken( Operator.Colon, (0,0))
            };
            var reader = new TokenReader(tokens);
            var analyzer = new SyntacticAnalyzer(reader);

            // Act
            var success = analyzer.TryExpressionStatement(out ExpressionStatementAst expressionStatement);

            // Assert
            Assert.False(success);
            Assert.Null(expressionStatement);

            Assert.Equal(0, reader._cursor);
        }

        [Fact]
        void ConstDeclaration_returns_correct_ast_and_advance_when_success()
        {
            // const_decl_stmt -> 'const' IDENT ':' ty '=' expr ';'

            // Arrange
            var tokens = new Token[]
            {
                new KeywordToken( Keyword.Const,(0,0)),
                new IdentifierToken("pi",(0,0)),
                new OperatorToken(Operator.Colon,(0,0)),
                new IdentifierToken("int",(0,0)),
                new OperatorToken(Operator.Assign, default),
                new DoubleLiteralToken(3.14159,(0,0)),
                new OperatorToken(Operator.Semicolon, default),
            };
            var reader = new TokenReader(tokens);
            var analyzer = new SyntacticAnalyzer(reader);

            // Act
            var success = analyzer.TryConstDeclarationStatement(out ConstDeclarationStatement constDeclaration);

            // Assert
            Assert.True(success);
            Assert.NotNull(constDeclaration);

            Assert.True(reader.ReachedEnd);

            Assert.Equal(tokens[0], constDeclaration.Const);
            Assert.Equal(tokens[1], constDeclaration.Name);
            Assert.Equal(tokens[2], constDeclaration.Colon);
            Assert.Equal(tokens[3], constDeclaration.Type);
            Assert.Equal(tokens[4], constDeclaration.Assign);
            Assert.Equal(3.14159, constDeclaration.ValueExpression.Calculate());
            Assert.Equal(tokens[6], constDeclaration.Semicolon);
        }

        [Fact]
        void LetDeclaration_returns_correct_ast_and_advance_when_success_has_init()
        {
            // 'let' IDENT ':' ty ('=' expr)? ';'

            // Arrange
            var tokens = new Token[]
            {
                new KeywordToken( Keyword.Let,(0,0)),
                new IdentifierToken("count",(0,0)),
                new OperatorToken(Operator.Colon,(0,0)),
                new IdentifierToken("int",(0,0)),
                new OperatorToken(Operator.Assign, default),
                new UInt64LiteralToken(555,(0,0)),
                new OperatorToken(Operator.Semicolon, default),
            };
            var reader = new TokenReader(tokens);
            var analyzer = new SyntacticAnalyzer(reader);

            // Act
            var success = analyzer.TryLetDeclarationStatement(out LetDeclarationStatementAst letDeclaration);

            // Assert
            Assert.True(success);
            Assert.NotNull(letDeclaration);

            Assert.True(reader.ReachedEnd);

            Assert.Equal(tokens[0], letDeclaration.Let);
            Assert.Equal(tokens[1], letDeclaration.Name);
            Assert.Equal(tokens[2], letDeclaration.Colon);
            Assert.Equal(tokens[3], letDeclaration.Type);
            Assert.Equal(tokens[4], letDeclaration.Assign);
            Assert.Equal(555, letDeclaration.InitialExpression.Calculate());
            Assert.Equal(tokens[6], letDeclaration.Semicolon);
        }

        [Fact]
        void LetDeclaration_returns_correct_ast_and_advance_when_success_no_init()
        {
            // 'let' IDENT ':' ty ('=' expr)? ';'
            // 'let' IDENT ':' ty ';'

            // Arrange
            var tokens = new Token[]
            {
                new KeywordToken( Keyword.Let, default),
                new IdentifierToken("count", default),
                new OperatorToken(Operator.Colon, default),
                new IdentifierToken("int", default),
                new OperatorToken(Operator.Semicolon, default),
            };
            var reader = new TokenReader(tokens);
            var analyzer = new SyntacticAnalyzer(reader);

            // Act
            var success = analyzer.TryLetDeclarationStatement(out LetDeclarationStatementAst letDeclaration);

            // Assert
            Assert.True(success);
            Assert.NotNull(letDeclaration);

            Assert.True(reader.ReachedEnd);

            Assert.Equal(tokens[0], letDeclaration.Let);
            Assert.Equal(tokens[1], letDeclaration.Name);
            Assert.Equal(tokens[2], letDeclaration.Colon);
            Assert.Equal(tokens[3], letDeclaration.Type);
            Assert.Null(letDeclaration.Assign);
            Assert.Null(letDeclaration.InitialExpression);
            Assert.Equal(tokens[4], letDeclaration.Semicolon);
        }

        [Fact]
        void BlockStatement_success()
        {
            // 'let' IDENT ':' ty ('=' expr)? ';'
            // 'let' IDENT ':' ty ';'

            for (int statementCount = 0; statementCount < 5; statementCount++)
            {
                // Arrange
                var tokens = new List<Token>
                {
                    new OperatorToken(Operator.LeftBrace, default),
                };

                for (var i = 0; i < statementCount; i++)
                {
                    tokens.Add(new UInt64LiteralToken((ulong)(10 * i), default));
                    tokens.Add(new OperatorToken(Operator.Semicolon, default));
                }
                tokens.Add(new OperatorToken(Operator.RightBrace, default));

                var reader = new TokenReader(tokens);
                var analyzer = new SyntacticAnalyzer(reader);

                // Act
                var success = analyzer.TryBlockStatement(out BlockStatementAst blockStatement);

                // Assert
                Assert.True(success);
                Assert.NotNull(blockStatement);

                Assert.True(reader.ReachedEnd);

                Assert.Equal(tokens[0], blockStatement.LeftBrace);
                Assert.Equal(tokens[^1], blockStatement.RightBrace);

                var literals = tokens.Select(p => p as UInt64LiteralToken).Where(p => p != null).ToList();

                for (var i = 0; i < statementCount; i++)
                {
                    var intLiteral = literals[i];
                    var subStatement = (ExpressionStatementAst)blockStatement.Statements[i];
                    Assert.Equal((int)intLiteral.Value, subStatement.Expression.Calculate());
                }

            }
        }

        [Fact]
        void TryIfStatement_has_following_blockstmt_success()
        {
            // if_stmt -> 'if' expr block_stmt ('else' (block_stmt | if_stmt))?

            // Arrange
            var tokens = new Token[]
            {
                new KeywordToken(Keyword.If, default),
                new DoubleLiteralToken(3.5, (0, 0)),
                new OperatorToken(Operator.LeftBrace, (0, 0)),
                new OperatorToken(Operator.RightBrace, (0, 0)),
                new KeywordToken(Keyword.Else, default),
                new OperatorToken(Operator.LeftBrace, (0, 0)),
                new OperatorToken(Operator.RightBrace, (0, 0)),

            };
            var reader = new TokenReader(tokens);
            var analyzer = new SyntacticAnalyzer(reader);

            // Act
            var success = analyzer.TryIfStatement(out IfStatementAst ifStatement);

            // Assert
            Assert.True(success);
            Assert.NotNull(ifStatement);

            Assert.True(reader.ReachedEnd);

            Assert.Equal(tokens[0], ifStatement.If);
            Assert.Equal(3.5, ifStatement.ConditionExpression.Calculate());
            Assert.Equal(tokens[2], ifStatement.BlockStatement.LeftBrace);
            Assert.Equal(tokens[3], ifStatement.BlockStatement.RightBrace);
            Assert.Equal(tokens[4], ifStatement.Else);

            Assert.True(ifStatement.HasElseAndFollowing);
            Assert.NotNull(ifStatement.FollowingBlock);
            Assert.Null(ifStatement.FollowingIf);
            Assert.Equal(tokens[5], ifStatement.FollowingBlock.LeftBrace);
            Assert.Equal(tokens[6], ifStatement.FollowingBlock.RightBrace);
        }

        [Fact]
        void TryIfStatement_has_following_ifstmt_success()
        {
            // if_stmt -> 'if' expr block_stmt ('else' (block_stmt | if_stmt))?

            // Arrange
            var tokens = new Token[]
            {
                new KeywordToken(Keyword.If, default),
                new DoubleLiteralToken(3.5, (0, 0)),
                new OperatorToken(Operator.LeftBrace, (0, 0)),
                new OperatorToken(Operator.RightBrace, (0, 0)),
                new KeywordToken(Keyword.Else, default),
                new KeywordToken(Keyword.If, default),
                new DoubleLiteralToken(3.09, (0, 0)),
                new OperatorToken(Operator.LeftBrace, (0, 0)),
                new OperatorToken(Operator.RightBrace, (0, 0)),
            };
            var reader = new TokenReader(tokens);
            var analyzer = new SyntacticAnalyzer(reader);

            // Act
            var success = analyzer.TryIfStatement(out IfStatementAst ifStatement);

            // Assert
            Assert.True(success);
            Assert.NotNull(ifStatement);

            Assert.True(reader.ReachedEnd);

            Assert.Equal(tokens[0], ifStatement.If);
            Assert.Equal(3.5, ifStatement.ConditionExpression.Calculate());
            Assert.Equal(tokens[2], ifStatement.BlockStatement.LeftBrace);
            Assert.Equal(tokens[3], ifStatement.BlockStatement.RightBrace);
            Assert.Equal(tokens[4], ifStatement.Else);

            Assert.True(ifStatement.HasElseAndFollowing);
            Assert.Null(ifStatement.FollowingBlock);
            Assert.NotNull(ifStatement.FollowingIf);
            Assert.Equal(tokens[5], ifStatement.FollowingIf.If);
            Assert.Equal(3.09, ifStatement.FollowingIf.ConditionExpression.Calculate());
            Assert.Equal(tokens[7], ifStatement.FollowingIf.BlockStatement.LeftBrace);
            Assert.Equal(tokens[8], ifStatement.FollowingIf.BlockStatement.RightBrace);
        }

        [Fact]
        void TryIfStatement_no_following_success()
        {
            // if_stmt -> 'if' expr block_stmt ('else' (block_stmt | if_stmt))?

            // Arrange
            var tokens = new Token[]
            {
                new KeywordToken(Keyword.If, default),
                new DoubleLiteralToken(3.5, (0, 0)),
                new OperatorToken(Operator.LeftBrace, (0, 0)),
                new OperatorToken(Operator.RightBrace, (0, 0)),
            };
            var reader = new TokenReader(tokens);
            var analyzer = new SyntacticAnalyzer(reader);

            // Act
            var success = analyzer.TryIfStatement(out IfStatementAst ifStatement);

            // Assert
            Assert.True(success);
            Assert.NotNull(ifStatement);

            Assert.True(reader.ReachedEnd);

            Assert.Equal(tokens[0], ifStatement.If);
            Assert.Equal(3.5, ifStatement.ConditionExpression.Calculate());

            Assert.False(ifStatement.HasElseAndFollowing);
            Assert.Null(ifStatement.FollowingBlock);
            Assert.Null(ifStatement.FollowingIf);
        }

        [Fact]
        void WhileStatement_success()
        {
            // while_stmt -> 'while' expr block_stmt

            // Arrange
            var tokens = new Token[]
            {
                new KeywordToken(Keyword.While, default),
                new DoubleLiteralToken(66.67, (0, 0)),
                new OperatorToken(Operator.LeftBrace, (0, 0)),
                new OperatorToken(Operator.RightBrace, (0, 0)),
            };
            var reader = new TokenReader(tokens);
            var analyzer = new SyntacticAnalyzer(reader);

            // Act
            var success = analyzer.TryWhileStatement(out WhileStatementAst whileStatement);

            // Assert
            Assert.True(success);
            Assert.NotNull(whileStatement);

            Assert.True(reader.ReachedEnd);

            Assert.Equal(tokens[0], whileStatement.While);
            Assert.Equal(66.67, whileStatement.ConditionExpression.Calculate());
            Assert.Equal(tokens[2], whileStatement.WhileBlock.LeftBrace);
            Assert.Equal(tokens[3], whileStatement.WhileBlock.RightBrace);
        }

        [Fact]
        void ReturnStatement_valued_success()
        {
            // return_stmt -> 'return' expr? ';'

            // Arrange
            var tokens = new Token[]
            {
                new KeywordToken(Keyword.Return, default),
                new DoubleLiteralToken(6657.0, (0, 0)),
                new OperatorToken(Operator.Semicolon, (0, 0)),
            };
            var reader = new TokenReader(tokens);
            var analyzer = new SyntacticAnalyzer(reader);

            // Act
            var success = analyzer.TryReturnStatement(out ReturnStatementAst returnStatement);

            // Assert
            Assert.True(success);
            Assert.NotNull(returnStatement);

            Assert.True(reader.ReachedEnd);

            Assert.Equal(tokens[0], returnStatement.Return);
            Assert.Equal(6657.0, returnStatement.ReturnExpression.Calculate());
            Assert.Equal(tokens[2], returnStatement.Semicolon);
        }

        [Fact]
        void ReturnStatement_empty_success()
        {
            // return_stmt -> 'return' expr? ';'

            // Arrange
            var tokens = new Token[]
            {
                new KeywordToken(Keyword.Return, default),
                new OperatorToken(Operator.Semicolon, (0, 0)),
            };
            var reader = new TokenReader(tokens);
            var analyzer = new SyntacticAnalyzer(reader);

            // Act
            var success = analyzer.TryReturnStatement(out ReturnStatementAst returnStatement);

            // Assert
            Assert.True(success);
            Assert.NotNull(returnStatement);

            Assert.True(reader.ReachedEnd);

            Assert.Equal(tokens[0], returnStatement.Return);
            Assert.Null(returnStatement.ReturnExpression);
            Assert.Equal(tokens[1], returnStatement.Semicolon);
        }

        [Fact]
        void EmptyStatement_success()
        {
            // ;

            // Arrange
            var tokens = new Token[]
            {
                new OperatorToken(Operator.Semicolon, (0, 0)),
            };
            var reader = new TokenReader(tokens);
            var analyzer = new SyntacticAnalyzer(reader);

            // Act
            var success = analyzer.TryEmptyStatement(out EmptyStatementAst emptyStatement);

            // Assert
            Assert.True(success);
            Assert.NotNull(emptyStatement);

            Assert.True(reader.ReachedEnd);

            Assert.Equal(tokens[0], emptyStatement.Semicolon);
        }

        [Fact]
        void BreakStatement_success()
        {
            // break_stmt -> 'break' ';'

            // Arrange
            var tokens = new Token[]
            {
                new KeywordToken(Keyword.Break, default),
                new OperatorToken(Operator.Semicolon, (0, 0)),
            };
            var reader = new TokenReader(tokens);
            var analyzer = new SyntacticAnalyzer(reader);

            // Act
            var success = analyzer.TryBreakStatement(out BreakStatementAst breakStatement);

            // Assert
            Assert.True(success);
            Assert.NotNull(breakStatement);

            Assert.True(reader.ReachedEnd);

            Assert.Equal(tokens[0], breakStatement.Break);
            Assert.Equal(tokens[1], breakStatement.Semicolon);
        }

        [Fact]
        void ContinueStatement_success()
        {
            // continue_stmt -> 'continue' ';'

            // Arrange
            var tokens = new Token[]
            {
                new KeywordToken(Keyword.Continue, default),
                new OperatorToken(Operator.Semicolon, (0, 0)),
            };
            var reader = new TokenReader(tokens);
            var analyzer = new SyntacticAnalyzer(reader);

            // Act
            var success = analyzer.TryContinueStatement(out ContinueStatementAst continueStatement);

            // Assert
            Assert.True(success);
            Assert.NotNull(continueStatement);

            Assert.True(reader.ReachedEnd);

            Assert.Equal(tokens[0], continueStatement.Continue);
            Assert.Equal(tokens[1], continueStatement.Semicolon);
        }
    }
}
