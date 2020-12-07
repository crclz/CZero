using CZero.Intermediate.Symbols;
using CZero.Lexical.Tokens;
using CZero.Syntactic.Ast;
using CZero.Syntactic.Ast.Expressions;
using CZero.Syntactic.Ast.Expressions.OperatorExpression;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace CZero.Intermediate.Test.SemanticCheck
{
    public class ExpressionTest
    {
        static IntermediateCodeGenerator ConfigureGenerator(
            SymbolScope symbolScope,
            Action<Mock<IntermediateCodeGenerator>> action)
        {
            var mock = new Mock<IntermediateCodeGenerator>(symbolScope);
            mock.CallBase = true;

            action(mock);

            return mock.Object;
        }

        [Fact]
        void ProcessAssignExpression_throws_when_name_is_not_symbol()
        {
            var scope = new SymbolScope();

            var name = new IdentifierToken("today", default);

            var ast = new AssignExpressionAst(
                name,
                new OperatorToken(Operator.Assign, default),
                new Mock<ExpressionAst>().Object);

            var generator = ConfigureGenerator(scope, mock =>
             {
             });

            Assert.Throws<SemanticException>(() => generator.ProcessAssignExpression(ast));
        }

        [Fact]
        void ProcessAssignExpression_throws_when_symbol_is_function()
        {
            var scope = new SymbolScope();

            var name = new IdentifierToken("today", default);
            var funcSymbol = new FunctionSymbol(name.Value, DataType.Long, new DataType[0]);
            scope.AddSymbol(funcSymbol);

            var ast = new AssignExpressionAst(
                name,
                new OperatorToken(Operator.Assign, default),
                new Mock<ExpressionAst>().Object);

            var generator = ConfigureGenerator(scope, mock =>
            {
            });

            Assert.Throws<SemanticException>(() => generator.ProcessAssignExpression(ast));
        }

        [Fact]
        void ProcessAssignExpression_throws_when_symbol_is_constant()
        {
            var scope = new SymbolScope();

            var name = new IdentifierToken("today", default);
            var constSymbol = new VariableSymbol(name.Value, true, true, DataType.Double, 1.23);
            scope.AddSymbol(constSymbol);

            var ast = new AssignExpressionAst(
                name,
                new OperatorToken(Operator.Assign, default),
                new Mock<ExpressionAst>().Object);

            var generator = ConfigureGenerator(scope, mock =>
            {
            });

            Assert.Throws<SemanticException>(() => generator.ProcessAssignExpression(ast));
        }

        [Fact]
        void ProcessAssignExpression_throws_when_expr_type_not_match_variable_type()
        {
            var scope = new SymbolScope();

            var name = new IdentifierToken("today", default);
            var constSymbol = new VariableSymbol(name.Value, true, isConstant: false, DataType.Double, 1.23);
            scope.AddSymbol(constSymbol);

            var expr = new Mock<ExpressionAst>().Object;

            var ast = new AssignExpressionAst(
                name,
                new OperatorToken(Operator.Assign, default),
                expr);

            var generator = ConfigureGenerator(scope, mock =>
            {
                mock.Setup(p => p.ProcessExpression(expr)).Returns(DataType.Long);
            });

            Assert.Throws<SemanticException>(() => generator.ProcessAssignExpression(ast));
        }

        [Fact]
        void ProcessAssignExpression_returns_DataTypeVoid_when_ok()
        {
            var scope = new SymbolScope();

            var name = new IdentifierToken("today", default);
            var constSymbol = new VariableSymbol(name.Value, true, isConstant: false, DataType.Double, 1.23);
            scope.AddSymbol(constSymbol);

            var expr = new Mock<ExpressionAst>().Object;

            var ast = new AssignExpressionAst(
                name,
                new OperatorToken(Operator.Assign, default),
                expr);

            var generator = ConfigureGenerator(scope, mock =>
            {
                mock.Setup(p => p.ProcessExpression(expr)).Returns(DataType.Double);
            });

            Assert.Equal(DataType.Void, generator.ProcessAssignExpression(ast));
        }
    }
}
