using CZero.Lexical.Tokens;
using CZero.Syntactic.Ast.Statements;
using CZero.Syntactic.Ast.Statements.Declarative;
using System;
using System.Collections.Generic;
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
    }
}
