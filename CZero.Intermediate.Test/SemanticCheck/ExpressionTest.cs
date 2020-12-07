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


        #region Call Expression

        [Fact]
        void ProcessCallExpression_throws_when_symbol_not_exist()// TODO: 考虑系统调用
        {
            var scope = new SymbolScope();

            var name = new IdentifierToken("print", default);

            var ast = new CallExpressionAst(
                name, new OperatorToken(Operator.LeftParen, default), null,
                new OperatorToken(Operator.RightParen, default));

            var generator = ConfigureGenerator(scope, mock =>
            {
            });

            Assert.Throws<SemanticException>(() => generator.ProcessCallExpression(ast));
        }

        [Fact]
        void ProcessCallExpression_throws_when_symbol_is_not_function()
        {
            var scope = new SymbolScope();

            var name = new IdentifierToken("print", default);

            var varSymbol = new VariableSymbol(name.Value, true, false, DataType.Long, 321L);
            scope.AddSymbol(varSymbol);

            var ast = new CallExpressionAst(
                name, new OperatorToken(Operator.LeftParen, default), null,
                new OperatorToken(Operator.RightParen, default));

            var generator = ConfigureGenerator(scope, mock =>
            {
            });

            Assert.Throws<SemanticException>(() => generator.ProcessCallExpression(ast));
        }


        [Fact]
        void ProcessCallExpression_throw_when_not_null_param_to_call_0_param_function()
        {
            var scope = new SymbolScope();

            var name = new IdentifierToken("print", default);

            var funcSymbol = new FunctionSymbol(name.Value, DataType.Long, new DataType[0]);
            scope.AddSymbol(funcSymbol);

            var param1 = new Mock<ExpressionAst>().Object;
            var callParamList = new CallParamListAst(new[] { param1 });

            var ast = new CallExpressionAst(
                name, new OperatorToken(Operator.LeftParen, default), callParamList,
                new OperatorToken(Operator.RightParen, default));

            var generator = ConfigureGenerator(scope, mock =>
            {
                mock.Setup(p => p.ProcessExpression(param1)).Returns(DataType.Long);
            });

            Assert.Throws<SemanticException>(() => generator.ProcessCallExpression(ast));
        }

        [Fact]
        void ProcessCallExpression_throw_when_provide_no_param_to_call_function_need_param()
        {
            var scope = new SymbolScope();

            var name = new IdentifierToken("print", default);

            var funcSymbol = new FunctionSymbol(name.Value, DataType.Long, new DataType[] { DataType.Long });
            scope.AddSymbol(funcSymbol);

            var ast = new CallExpressionAst(
                name, new OperatorToken(Operator.LeftParen, default), null,
                new OperatorToken(Operator.RightParen, default));

            var generator = ConfigureGenerator(scope, mock =>
            {
            });

            Assert.Throws<SemanticException>(() => generator.ProcessCallExpression(ast));
        }

        // Param number not match
        [Fact]
        void ProcessCallExpression_throw_when_param_number_not_match()
        {
            var scope = new SymbolScope();

            var name = new IdentifierToken("print", default);

            var funcSymbol = new FunctionSymbol(name.Value, DataType.Long, new DataType[] { DataType.Long });
            scope.AddSymbol(funcSymbol);

            var param1 = new Mock<ExpressionAst>().Object;
            var param2 = new Mock<ExpressionAst>().Object;
            var callParamList = new CallParamListAst(new[] { param1, param2 });

            var ast = new CallExpressionAst(
                name, new OperatorToken(Operator.LeftParen, default), callParamList,
                new OperatorToken(Operator.RightParen, default));

            var generator = ConfigureGenerator(scope, mock =>
            {
                mock.Setup(p => p.ProcessExpression(param1)).Returns(DataType.Long);
                mock.Setup(p => p.ProcessExpression(param2)).Returns(DataType.Long);
            });

            Assert.Throws<SemanticException>(() => generator.ProcessCallExpression(ast));
        }

        [Fact]
        void ProcessCallExpression_throw_when_param_type_not_match()
        {
            var scope = new SymbolScope();

            var name = new IdentifierToken("print", default);

            var funcSymbol = new FunctionSymbol(name.Value, DataType.Long,
                new DataType[] { DataType.Long, DataType.Long });// need
            scope.AddSymbol(funcSymbol);

            var param1 = new Mock<ExpressionAst>().Object;
            var param2 = new Mock<ExpressionAst>().Object;
            var callParamList = new CallParamListAst(new[] { param1, param2 });

            var ast = new CallExpressionAst(
                name, new OperatorToken(Operator.LeftParen, default), callParamList,
                new OperatorToken(Operator.RightParen, default));

            var generator = ConfigureGenerator(scope, mock =>
            {
                mock.Setup(p => p.ProcessExpression(param1)).Returns(DataType.Long);// provide
                mock.Setup(p => p.ProcessExpression(param2)).Returns(DataType.Double);// provide
            });

            Assert.Throws<SemanticException>(() => generator.ProcessCallExpression(ast));
        }

        [Fact]
        void ProcessCallExpression_returns_returnType_when_ok()
        {
            var scope = new SymbolScope();

            var name = new IdentifierToken("print", default);

            var funcSymbol = new FunctionSymbol(name.Value, DataType.Long,// return
                new DataType[] { DataType.Double, DataType.Double });// need
            scope.AddSymbol(funcSymbol);

            var param1 = new Mock<ExpressionAst>().Object;
            var param2 = new Mock<ExpressionAst>().Object;
            var callParamList = new CallParamListAst(new[] { param1, param2 });

            var ast = new CallExpressionAst(
                name, new OperatorToken(Operator.LeftParen, default), callParamList,
                new OperatorToken(Operator.RightParen, default));

            var generator = ConfigureGenerator(scope, mock =>
            {
                mock.Setup(p => p.ProcessExpression(param1)).Returns(DataType.Double);// provide
                mock.Setup(p => p.ProcessExpression(param2)).Returns(DataType.Double);// provide
            });

            Assert.Equal(DataType.Long, generator.ProcessCallExpression(ast));
        }

        [Fact]
        void ProcessCallExpression_returns_returnType_when_0_0()
        {
            var scope = new SymbolScope();

            var name = new IdentifierToken("print", default);

            var funcSymbol = new FunctionSymbol(name.Value, DataType.Double,// return
                new DataType[] { });// need
            scope.AddSymbol(funcSymbol);

            var ast = new CallExpressionAst(
                name, new OperatorToken(Operator.LeftParen, default), null,
                new OperatorToken(Operator.RightParen, default));

            var generator = ConfigureGenerator(scope, mock =>
            {
            });

            Assert.Equal(DataType.Double, generator.ProcessCallExpression(ast));
        }

        #endregion


        #region IdentExpression

        [Fact]
        void ProcessIdentExpression_throws_when_symbol_not_found()
        {
            var scope = new SymbolScope();

            var name = new IdentifierToken("aCounter", default);

            var ast = new IdentExpressionAst(name);

            var generator = ConfigureGenerator(scope, mock =>
            {
            });

            Assert.Throws<SemanticException>(() => generator.ProcessIdentExpression(ast));
        }

        [Fact]
        void ProcessIdentExpression_throws_when_symbol_not_var_or_const()
        {
            var scope = new SymbolScope();
            var funcSymbol = new FunctionSymbol("aCounter", DataType.Void, new DataType[0]);
            scope.AddSymbol(funcSymbol);

            var name = new IdentifierToken("aCounter", default);

            var ast = new IdentExpressionAst(name);

            var generator = ConfigureGenerator(scope, mock =>
            {
            });

            Assert.Throws<SemanticException>(() => generator.ProcessIdentExpression(ast));
        }

        [Fact]
        void ProcessIdentExpression_returns_type_when_constvar_ok()
        {
            var scope = new SymbolScope();
            var funcSymbol = new VariableSymbol("aCounter", true, false, DataType.Long, null);
            scope.AddSymbol(funcSymbol);

            var name = new IdentifierToken("aCounter", default);

            var ast = new IdentExpressionAst(name);

            var generator = ConfigureGenerator(scope, mock =>
            {
            });

            Assert.Equal(DataType.Long, generator.ProcessIdentExpression(ast));
        }

        #endregion
    }
}
