using CZero.Intermediate.Builders;
using CZero.Intermediate.Instructions;
using CZero.Intermediate.Symbols;
using CZero.Lexical.Tokens;
using CZero.Syntactic.Ast;
using CZero.Syntactic.Ast.Expressions;
using CZero.Syntactic.Ast.Expressions.OperatorExpression;
using CZero.Syntactic.Ast.Statements;
using CZero.Syntactic.Ast.Statements.Declarative;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace CZero.Intermediate.Test.CodeGen
{
    public class StatementTest
    {
        static IntermediateCodeGenerator ConfigureGenerator(
            SymbolScope symbolScope,
            Action<Mock<IntermediateCodeGenerator>> action)
        {
            var mock = new Mock<IntermediateCodeGenerator>(symbolScope);
            mock.CallBase = true;

            action(mock);

            var g = mock.Object;
            g.CodeGenerationEnabled = true;

            return g;
        }

        static void AssertJsonEqual(object a, object b)
        {
            var typeA = a.GetType();
            Assert.IsType(typeA, b);

            var aJson = Newtonsoft.Json.JsonConvert.SerializeObject(a);
            var bJson = Newtonsoft.Json.JsonConvert.SerializeObject(b);
            Assert.Equal(aJson, bJson);
        }

        [Fact]
        void ExpressionStatement_generates_nothing_if_expr_returning_void()// this is not correct!!!
        {
            // Arrange
            var scope = new SymbolScope();

            var expr = new Mock<ExpressionAst>().Object;


            var generator = ConfigureGenerator(scope, mock =>
            {
                mock.Setup(p => p.ProcessExpression(expr)).Returns(DataType.Void);
            });

            var function = new FunctionSymbol("p", DataType.Void, new DataType[0]);
            generator.GlobalBuilder.RegisterFunction(function);
            generator.EnterFunctionDefination(function);

            var ast = new ExpressionStatementAst(expr, new OperatorToken(Operator.Semicolon, default));

            // Act
            generator.ProcessExpressionStatement(ast);

            // Assert
            var instructions = generator.CurrentFunction.Builder.Bucket.Pop();
            Assert.Empty(instructions);
        }

        [Fact]
        void ExpressionStatement_generates_pop_if_expr_not_returning_void()// not correct!!!
        {
            // Arrange
            var scope = new SymbolScope();

            var expr = new Mock<ExpressionAst>().Object;

            var generator = ConfigureGenerator(scope, mock =>
            {
                mock.Setup(p => p.ProcessExpression(expr)).Returns(DataType.Long);
            });

            var ast = new ExpressionStatementAst(expr, new OperatorToken(Operator.Semicolon, default));

            var function = new FunctionSymbol("p", DataType.Void, new DataType[0]);
            generator.GlobalBuilder.RegisterFunction(function);
            generator.EnterFunctionDefination(function);

            // Act
            generator.ProcessExpressionStatement(ast);

            // Assert
            var instructions = generator.CurrentFunction.Builder.Bucket.Pop();
            Assert.Single(instructions);
            Assert.Equal("pop", instructions[0].Parts.Single());

        }

        [Fact]
        void ProcessLetDeclaration_registers_and_set_instructions_when_out_of_func()
        {
            // Arrange
            var expr = new Mock<ExpressionAst>().Object;
            var ast = new LetDeclarationStatementAst(
                new KeywordToken(Keyword.Let, default), new IdentifierToken("x", default),
                new OperatorToken(Operator.Colon, default), new IdentifierToken("int", default),
                new OperatorToken(Operator.Assign, default), expr,
                new OperatorToken(Operator.Semicolon, default));

            var scope = new SymbolScope();
            var generator = ConfigureGenerator(scope, mock =>
            {
                mock.Setup(p => p.ProcessExpression(expr)).Returns(DataType.Long);
            });

            var ins1 = new Instruction(new object[] { "push", 111L });
            generator.ExpressionBucket.Add(ins1);// this is initial value expr

            // Act
            generator.ProcessLetDeclarationStatement(ast);

            // Assert
            Assert.Empty(generator.ExpressionBucket.Pop());

            Assert.True(scope.FindSymbolShallow("x", out Symbol symbol));

            var varx = Assert.IsType<VariableSymbol>(symbol);

            Assert.Contains(varx, generator.GlobalBuilder.GlobalVariablesView);

            Assert.NotNull(varx.GlobalVariableBuilder);
            Assert.Single(varx.GlobalVariableBuilder.LoadValueInstructions);
            Assert.Equal(ins1, varx.GlobalVariableBuilder.LoadValueInstructions.Single());
        }

        [Fact]
        void ProcessLetDeclaration_registers_to_function_and_writes_function_bucket_when_in_func()
        {
            // Arrange
            var expr = new Mock<ExpressionAst>().Object;
            var ast = new LetDeclarationStatementAst(
                new KeywordToken(Keyword.Let, default), new IdentifierToken("x", default),
                new OperatorToken(Operator.Colon, default), new IdentifierToken("int", default),
                new OperatorToken(Operator.Assign, default), expr,
                new OperatorToken(Operator.Semicolon, default));

            var function = new FunctionSymbol("print", DataType.Long, new DataType[0]);

            var scope = new SymbolScope();
            var generator = ConfigureGenerator(scope, mock =>
            {
                mock.Setup(p => p.ProcessExpression(expr)).Returns(DataType.Long);
            });

            generator.GlobalBuilder.RegisterFunction(function);
            generator.EnterFunctionDefination(function);

            var ins1 = new object[] { "push", 111L };
            generator.ExpressionBucket.Add(ins1);// this is initial value expr

            // Act
            generator.ProcessLetDeclarationStatement(ast);

            // Assert
            Assert.Empty(generator.ExpressionBucket.Pop());

            Assert.True(scope.FindSymbolShallow("x", out Symbol symbol));

            var varx = Assert.IsType<VariableSymbol>(symbol);

            Assert.Contains(varx, function.Builder.LocalVariables);

            Assert.NotNull(varx.LocalLocation);
            Assert.Equal(0, varx.LocalLocation.Id);
            Assert.False(varx.LocalLocation.IsArgument);

            var funcBody = function.Builder.Bucket.InstructionList;
            Assert.Equal(3, funcBody.Count);
            AssertJsonEqual(new object[] { "loca", 0 }, funcBody[0].Parts);
            AssertJsonEqual(ins1, funcBody[1].Parts);
            AssertJsonEqual(new object[] { "store.64" }, funcBody[2].Parts);
        }
    }
}
