using CZero.Lexical.Tokens;
using CZero.Syntactic.Ast.Expressions;
using CZero.Syntactic.Ast.Expressions.OperatorExpression;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace CZero.Syntactic.Test.AnalyzerTest
{
    /*
    测试计划

    分为多阶段。每个阶段启用一点东西。
    在每个阶段中，先单元测试，然后集成测试。

    expr -> 
        | operator_exp
        | negate_expr   禁用，下一步就启用
        | assign_expr   禁用
        | call_expr     逐步启用，解释器设置几个专用函数调用的结果计算
        | literal_expr  
        | ident_expr    禁用
        | group_expr

    */

    /*
    Stage 1

    expr -> 
        | operator_exp
        | negate_expr   禁用
        | assign_expr   禁用
        | call_expr     禁用
        | literal_expr  
        | ident_expr    禁用
        | group_expr

    weak_term -> term { +|- term }
    term -> factor { *|/ factor }
    factor -> strong_factor { as ty} // ty -> IDENT
    strong_factor -> negate_expr | assign_expr | call_expr | literal_expr | ident_expr | group_expr

    */

    public class OperatorExpressionTestStage1
    {
        private Mock<SyntacticAnalyzer> Configure(Mock<SyntacticAnalyzer> mock)
        {
            NegateExpressionAst negateExpression = null;
            AssignExpressionAst assignExpression = null;
            CallExpressionAst callExpression = null;
            IdentExpressionAst identExpression = null;

            mock.Setup(p => p.TryNegateExpression(out negateExpression)).Returns(false);
            mock.Setup(p => p.TryAssignExpression(out assignExpression)).Returns(false);
            mock.Setup(p => p.TryCallExpression(out callExpression)).Returns(false);
            mock.Setup(p => p.TryIdentExpression(out identExpression)).Returns(false);

            return mock;
        }

        public OperatorExpressionTestStage1()
        {
        }

        [Fact]
        void LiteralExpressionSuccess()
        {
            var tokenA = new UInt64LiteralToken(1, (2, 2));
            var reader = new TokenReader(new Token[] { tokenA });
            var analyzer = Configure(new Mock<SyntacticAnalyzer>(reader)).Object;

            var success = analyzer.TryLiteralExpression(out LiteralExpressionAst literalExpression);

            Assert.True(success);
            Assert.NotNull(literalExpression);
            Assert.Equal(tokenA, literalExpression.Literal);

            Assert.True(reader.ReachedEnd);
        }

        [Fact]
        void LiteralExpressionFail()
        {
            var tokenA = new IdentifierToken("alpha", (2, 2));
            var reader = new TokenReader(new Token[] { tokenA });
            var analyzer = Configure(new Mock<SyntacticAnalyzer>(reader)).Object;

            var success = analyzer.TryLiteralExpression(out LiteralExpressionAst literalExpression);
            Assert.False(success);
            Assert.Null(literalExpression);

            Assert.Equal(0, reader._cursor);
        }

        [Fact]
        void GroupExpressionSuccess()
        {
            // Arrange
            var lParen = new OperatorToken(Operator.LeftParen, (0, 0));
            var middle = new UInt64LiteralToken(1, (0, 0));
            var rParen = new OperatorToken(Operator.RightParen, (0, 0));

            var reader = new TokenReader(new Token[] { lParen, middle, rParen });
            var analyzer = Configure(new Mock<SyntacticAnalyzer>(reader)).Object;

            // Act
            var success = analyzer.TryGroupExpression(out GroupExpressionAst groupExpression);

            // Assert
            Assert.True(success);
            Assert.NotNull(groupExpression);
            Assert.Equal(lParen, groupExpression.LeftParen);
            Assert.Equal(rParen, groupExpression.RightParen);
            var literal = Assert.IsType<LiteralExpressionAst>(groupExpression.Expression);
            Assert.Equal(middle, literal.Literal);
            Assert.True(reader.ReachedEnd);
        }

        [Fact]
        void GroupExpressionFailure()
        {
            // Arrange
            var lParen = new OperatorToken(Operator.LeftParen, (0, 0));
            var middle = new UInt64LiteralToken(1, (0, 0));

            var reader = new TokenReader(new Token[] { lParen, middle });
            var analyzer = Configure(new Mock<SyntacticAnalyzer>(reader)).Object;

            // Act
            var success = analyzer.TryGroupExpression(out GroupExpressionAst groupExpression);

            // Assert
            Assert.False(success);
            Assert.Null(groupExpression);

            Assert.Equal(0, reader._cursor);
        }

        [Fact]
        void StrongFactorSuccess()
        {
            var tokenA = new UInt64LiteralToken(1, (2, 2));
            var reader = new TokenReader(new Token[] { tokenA });
            var analyzer = Configure(new Mock<SyntacticAnalyzer>(reader)).Object;

            var success = analyzer.TryStrongFactor(out StrongFactorAst strongFactor);

            Assert.True(success);
            Assert.NotNull(strongFactor);

            var literal = Assert.IsType<LiteralExpressionAst>(strongFactor.SingleExpression);
            Assert.Equal(tokenA, literal.Literal);

            Assert.True(reader.ReachedEnd);
        }

        [Fact]
        void StrongFactorFail()
        {
            var LParen = new OperatorToken(Operator.LeftParen, (0, 0));
            var tokenA = new UInt64LiteralToken(1, (2, 2));
            var reader = new TokenReader(new Token[] { LParen, tokenA });
            var analyzer = Configure(new Mock<SyntacticAnalyzer>(reader)).Object;

            var success = analyzer.TryStrongFactor(out StrongFactorAst strongFactor);

            Assert.False(success);
            Assert.Null(strongFactor);

            Assert.Equal(0, reader._cursor);
        }

        [Fact]
        void FactorZeroAsTest()
        {
            // Arrange
            var tokenA = new UInt64LiteralToken(1, (2, 2));
            var reader = new TokenReader(new Token[] { tokenA });
            var analyzer = Configure(new Mock<SyntacticAnalyzer>(reader)).Object;

            // Act
            var success = analyzer.TryFactor(out FactorAst factor);

            // Assert
            Assert.True(success);
            Assert.NotNull(factor);

            Assert.IsType<LiteralExpressionAst>(factor.StrongFactor.SingleExpression);

            Assert.True(reader.ReachedEnd);
        }

        [Fact]
        void FactorWithAsTypeTest()
        {
            // Arrange
            var tokenA = new UInt64LiteralToken(1, (0, 0));
            var asToken = new KeywordToken(Keyword.As, (0, 0));
            var typeA = new IdentifierToken("string", (0, 0));

            var reader = new TokenReader(new Token[] { tokenA, asToken, typeA });
            var analyzer = Configure(new Mock<SyntacticAnalyzer>(reader)).Object;

            // Act
            var success = analyzer.TryFactor(out FactorAst factor);

            // Assert
            Assert.True(success);
            Assert.True(reader.ReachedEnd);
            Assert.NotNull(factor);

            Assert.IsType<LiteralExpressionAst>(factor.StrongFactor.SingleExpression);

            // the as list
            Assert.Single(factor.AsTypeList);
            Assert.Equal(asToken, factor.AsTypeList[0].AsToken);
            Assert.Equal(typeA, factor.AsTypeList[0].TypeToken);
        }

        [Fact]
        void Factor_success_and_return_empty_list_when_meet_broken_as_list()
        {
            // Arrange
            var tokenA = new UInt64LiteralToken(1, (0, 0));
            var asToken = new KeywordToken(Keyword.As, (0, 0));
            var typeA = new KeywordToken(Keyword.Const, (0, 0));

            var reader = new TokenReader(new Token[] { tokenA, asToken, typeA });
            var analyzer = Configure(new Mock<SyntacticAnalyzer>(reader)).Object;

            // Act
            var success = analyzer.TryFactor(out FactorAst factor);

            // Assert
            Assert.True(success);
            Assert.Equal(1, reader._cursor);
            Assert.NotNull(factor);

            Assert.IsType<LiteralExpressionAst>(factor.StrongFactor.SingleExpression);

            // the as list
            Assert.Empty(factor.AsTypeList);
        }
    }
}
