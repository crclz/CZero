using CZero.Lexical.Tokens;
using CZero.Syntactic.Ast.Expressions;
using CZero.Syntactic.Ast.Expressions.OperatorExpression;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
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
        | assign_expr   禁用
        | call_expr     逐步启用，解释器设置几个专用函数调用的结果计算
        | literal_expr  
        | ident_expr    禁用
        | group_expr
        
    负值：没有负值的输入。这样分阶段是因为之前错误的文法导致的。

    */

    /*
    Stage 1

    expr -> 
        | operator_exp
        | assign_expr   禁用
        | call_expr     禁用
        | literal_expr  
        | ident_expr    禁用
        | group_expr

    operator_expr -> weak_term { 比较符 weak_term }
    weak_term -> term { +|- term }
    term -> factor { *|/ factor }
    factor -> good_factor { as ty} // ty -> IDENT
    good_factor -> { - } strong_factor
    strong_factor -> assign_expr | call_expr | literal_expr | ident_expr | group_expr

    */

    public partial class OperatorExpressionTestStage1
    {
        private Mock<SyntacticAnalyzer> Configure(Mock<SyntacticAnalyzer> mock)
        {
            mock.CallBase = true;

            AssignExpressionAst assignExpression = null;
            CallExpressionAst callExpression = null;
            IdentExpressionAst identExpression = null;

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
            //var literal = Assert.IsType<LiteralExpressionAst>(groupExpression.Expression);
            //Assert.Equal(middle, literal.Literal);
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

            Assert.IsType<LiteralExpressionAst>(factor.GoodFactor.StrongFactor.SingleExpression);

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

            Assert.IsType<LiteralExpressionAst>(factor.GoodFactor.StrongFactor.SingleExpression);

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

            Assert.IsType<LiteralExpressionAst>(factor.GoodFactor.StrongFactor.SingleExpression);

            // the as list
            Assert.Empty(factor.AsTypeList);
        }

        #region Term

        [Fact]
        void TermEmptyListTest()
        {
            // Arrange
            var tokenA = new UInt64LiteralToken(1, (2, 2));
            var reader = new TokenReader(new Token[] { tokenA });
            var analyzer = Configure(new Mock<SyntacticAnalyzer>(reader)).Object;

            // Act
            var success = analyzer.TryTerm(out TermAst term);

            // Assert
            Assert.True(success);
            Assert.NotNull(term);

            Assert.IsType<LiteralExpressionAst>(term.Factor.GoodFactor.StrongFactor.SingleExpression);

            Assert.True(reader.ReachedEnd);
        }

        [Fact]
        void TermWithListTest()
        {
            for (var listSize = 0; listSize < 10; listSize++)
            {
                // Arrange
                var tokenA = new UInt64LiteralToken(1, (0, 0));

                var tokens = new List<Token>();

                tokens.Add(tokenA);

                for (int i = 0; i < listSize; i++)
                {
                    var op = new OperatorToken(Operator.Divide, (0, 0));
                    var f = new DoubleLiteralToken(123.06, (0, 0));
                    tokens.Add(op);
                    tokens.Add(f);
                }

                var reader = new TokenReader(tokens);
                var analyzer = Configure(new Mock<SyntacticAnalyzer>(reader)).Object;

                // Act
                var success = analyzer.TryTerm(out TermAst term);

                // Assert
                Assert.True(success);
                Assert.True(reader.ReachedEnd);
                Assert.NotNull(term);

                Assert.IsType<LiteralExpressionAst>(term.Factor.GoodFactor.StrongFactor.SingleExpression);

                // the list
                Assert.Equal(listSize, term.OpFactors.Count);

                for (var i = 0; i < listSize; i++)
                {
                    var op = (OperatorToken)tokens[1 + 2 * i + 0];
                    var f = (LiteralToken)tokens[1 + 2 * i + 1];

                    Assert.Equal(op, term.OpFactors[i].Op);
                    Assert.Equal(f, (term.OpFactors[i].Factor.GoodFactor.StrongFactor.SingleExpression
                        as LiteralExpressionAst).Literal);
                }
            }
        }

        [Fact]
        void TermWithBrokenList()
        {
            // Arrange
            var tokenA = new UInt64LiteralToken(1, (0, 0));

            var tokens = new List<Token>();

            tokens.Add(tokenA);

            // list [0]
            var op = new OperatorToken(Operator.Divide, (0, 0));
            var f = new DoubleLiteralToken(123.06, (0, 0));
            tokens.Add(op);
            tokens.Add(f);

            // broken list [1]
            tokens.Add(op);

            var reader = new TokenReader(tokens);
            var analyzer = Configure(new Mock<SyntacticAnalyzer>(reader)).Object;

            // Act
            var success = analyzer.TryTerm(out TermAst term);

            // Assert
            Assert.True(success);
            Assert.False(reader.ReachedEnd);
            Assert.NotNull(term);

            Assert.IsType<LiteralExpressionAst>(term.Factor.GoodFactor.StrongFactor.SingleExpression);

            // the list
            Assert.Equal(1, term.OpFactors.Count);

            // list [0]
            Assert.Equal(op, term.OpFactors[0].Op);
            Assert.Equal(f, (term.OpFactors[0].Factor.GoodFactor.StrongFactor.SingleExpression as LiteralExpressionAst).Literal);
        }

        [Fact]
        void TermFail()
        {
            // Arrange
            var tokenA = new OperatorToken(Operator.GreaterEqual, (0, 0));

            var tokens = new List<Token>();

            tokens.Add(tokenA);

            // list [0]
            var op = new OperatorToken(Operator.Divide, (0, 0));
            var f = new DoubleLiteralToken(123.06, (0, 0));
            tokens.Add(op);
            tokens.Add(f);

            // broken list [1]
            tokens.Add(op);

            var reader = new TokenReader(tokens);
            var analyzer = Configure(new Mock<SyntacticAnalyzer>(reader)).Object;

            // Act
            var success = analyzer.TryTerm(out TermAst term);

            // Assert
            Assert.False(success);
            Assert.Null(term);
            Assert.Equal(0, reader._cursor);
        }

        #endregion



        #region WeakTerm

        [Fact]
        void WeakTermWithListTest()
        {
            for (var listSize = 0; listSize < 10; listSize++)
            {
                // Arrange
                var tokenA = new UInt64LiteralToken(1, (0, 0));

                var tokens = new List<Token>();

                tokens.Add(tokenA);

                for (int i = 0; i < listSize; i++)
                {
                    var op = new OperatorToken(Operator.Minus, (0, 0));
                    var f = new DoubleLiteralToken(123.06, (0, 0));
                    tokens.Add(op);
                    tokens.Add(f);
                }

                var reader = new TokenReader(tokens);
                var analyzer = Configure(new Mock<SyntacticAnalyzer>(reader)).Object;

                // Act
                var success = analyzer.TryWeakTerm(out WeakTermAst weakTerm);

                // Assert
                Assert.True(success);
                Assert.True(reader.ReachedEnd);
                Assert.NotNull(weakTerm);

                Assert.IsType<LiteralExpressionAst>(weakTerm.Term.Factor.GoodFactor.StrongFactor.SingleExpression);

                // the list
                Assert.Equal(listSize, weakTerm.OpTerms.Count);

                for (var i = 0; i < listSize; i++)
                {
                    var op = (OperatorToken)tokens[1 + 2 * i + 0];
                    var f = (LiteralToken)tokens[1 + 2 * i + 1];

                    Assert.Equal(op, weakTerm.OpTerms[i].Op);
                    Assert.Equal(f, (weakTerm.OpTerms[i].Term.Factor.GoodFactor.StrongFactor.SingleExpression
                        as LiteralExpressionAst).Literal);
                }
            }
        }

        [Fact]
        void WeakTermFailTest()
        {
            for (var listSize = 1; listSize < 10; listSize++)
            {
                // Arrange
                var tokenA = new UInt64LiteralToken(1, (0, 0));

                var tokens = new List<Token>();

                tokens.Add(tokenA);

                for (int i = 0; i < listSize; i++)
                {
                    var op = new OperatorToken(Operator.Minus, (0, 0));
                    var f = new DoubleLiteralToken(123.06, (0, 0));

                    tokens.Add(op);

                    if (i == listSize - 1)
                    {
                        // Make the last fail
                        tokens.Add(new KeywordToken(Keyword.If, (0, 0)));
                    }

                    tokens.Add(f);
                }

                var reader = new TokenReader(tokens);
                var analyzer = Configure(new Mock<SyntacticAnalyzer>(reader)).Object;

                // Act
                var success = analyzer.TryWeakTerm(out WeakTermAst weakTerm);

                // Assert
                Assert.True(success);

                Assert.False(reader.ReachedEnd);
                Assert.NotNull(weakTerm);


                Assert.IsType<LiteralExpressionAst>(weakTerm.Term.Factor.GoodFactor.StrongFactor.SingleExpression);

                // the list (the last is fail)
                Assert.Equal(listSize - 1, weakTerm.OpTerms.Count);

                for (var i = 0; i < listSize - 1; i++)
                {
                    var op = (OperatorToken)tokens[1 + 2 * i + 0];
                    var f = (LiteralToken)tokens[1 + 2 * i + 1];

                    Assert.Equal(op, weakTerm.OpTerms[i].Op);
                    Assert.Equal(f, (weakTerm.OpTerms[i].Term.Factor.GoodFactor.StrongFactor.SingleExpression
                        as LiteralExpressionAst).Literal);
                }
            }
        }

        #endregion


        #region OperatorExpression (very simple test)

        [Fact]
        void OperatorExpressionWithListTest()
        {
            for (var listSize = 0; listSize < 10; listSize++)
            {
                // Arrange
                var tokenA = new UInt64LiteralToken(1, (0, 0));

                var tokens = new List<Token>();

                tokens.Add(tokenA);

                for (int i = 0; i < listSize; i++)
                {
                    var op = new OperatorToken(Operator.GreaterEqual, (0, 0));
                    var f = new DoubleLiteralToken(123.06, (0, 0));
                    tokens.Add(op);
                    tokens.Add(f);
                }

                var reader = new TokenReader(tokens);
                var analyzer = Configure(new Mock<SyntacticAnalyzer>(reader)).Object;

                // Act
                var success = analyzer.TryOperatorExpression(out OperatorExpressionAst opExpr);

                // Assert
                Assert.True(success);
                Assert.True(reader.ReachedEnd);
                Assert.NotNull(opExpr);

                Assert.IsType<LiteralExpressionAst>(opExpr.WeakTerm.Term.Factor.GoodFactor.StrongFactor.SingleExpression);

                // the list
                Assert.Equal(listSize, opExpr.OpTerms.Count);

                for (var i = 0; i < listSize; i++)
                {
                    var op = (OperatorToken)tokens[1 + 2 * i + 0];
                    var f = (LiteralToken)tokens[1 + 2 * i + 1];

                    Assert.Equal(op, opExpr.OpTerms[i].Op);
                    Assert.Equal(f, (opExpr.OpTerms[i].WeakTerm.Term.Factor.GoodFactor.StrongFactor.SingleExpression
                        as LiteralExpressionAst).Literal);
                }
            }
        }

        [Fact]
        void OperatorExpressionBrokenListTest()
        {
            for (var listSize = 1; listSize < 10; listSize++)
            {
                // Arrange
                var tokenA = new UInt64LiteralToken(1, (0, 0));

                var tokens = new List<Token>();

                tokens.Add(tokenA);

                for (int i = 0; i < listSize; i++)
                {
                    var op = new OperatorToken(Operator.GreaterEqual, (0, 0));
                    var f = new DoubleLiteralToken(123.06, (0, 0));
                    tokens.Add(op);

                    // break the last item
                    if (i == listSize - 1)
                        tokens.Add(new KeywordToken(Keyword.Let, (0, 0)));

                    tokens.Add(f);
                }

                var reader = new TokenReader(tokens);
                var analyzer = Configure(new Mock<SyntacticAnalyzer>(reader)).Object;

                // Act
                var success = analyzer.TryOperatorExpression(out OperatorExpressionAst opExpr);

                // Assert
                Assert.True(success);
                Assert.False(reader.ReachedEnd);
                Assert.NotNull(opExpr);

                Assert.IsType<LiteralExpressionAst>(opExpr.WeakTerm.Term.Factor.GoodFactor.StrongFactor.SingleExpression);

                // the list
                Assert.Equal(listSize - 1, opExpr.OpTerms.Count);

                for (var i = 0; i < listSize - 1; i++)
                {
                    var op = (OperatorToken)tokens[1 + 2 * i + 0];
                    var f = (LiteralToken)tokens[1 + 2 * i + 1];

                    Assert.Equal(op, opExpr.OpTerms[i].Op);
                    Assert.Equal(f, (opExpr.OpTerms[i].WeakTerm.Term.Factor.GoodFactor.StrongFactor.SingleExpression
                        as LiteralExpressionAst).Literal);
                }
            }
        }

        #endregion
    }
}
