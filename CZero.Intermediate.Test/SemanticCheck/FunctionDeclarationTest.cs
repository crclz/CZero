using CZero.Intermediate.Builders;
using CZero.Intermediate.Symbols;
using CZero.Lexical.Tokens;
using CZero.Syntactic.Ast;
using CZero.Syntactic.Ast.Expressions;
using CZero.Syntactic.Ast.Expressions.OperatorExpression;
using CZero.Syntactic.Ast.Functions;
using CZero.Syntactic.Ast.Statements;
using CZero.Syntactic.Ast.Statements.Declarative;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace CZero.Intermediate.Test.SemanticCheck
{
    public class FunctionDeclarationTest
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

        // 函数的名称 function_name 不能重复，也不能和全局变量重复。
        // 函数的参数声明 param_list 与 含有初始化表达式的变量声明 有相同的语义约束。

        [Fact]
        void Function_throws_when_function_name_exist_deep()
        {
            var name = new IdentifierToken("SendRequest", default);

            var scope = new SymbolScope();
            scope.AddSymbol(new VariableSymbol(name.Value, true, false, DataType.Long));


            var ast = new FunctionAst(
                new KeywordToken(Keyword.Fn, default), name, new OperatorToken(Operator.LeftParen, default),
                functionParamList: null, new OperatorToken(Operator.RightParen, default),
                new OperatorToken(Operator.Arrow, default), new IdentifierToken("int", default),
                new Mock<BlockStatementAst>().Object
                );

            var generator = ConfigureGenerator(scope, mock =>
            {
            });

            Assert.Throws<SemanticException>(() => generator.ProcessFunction(ast));
        }

        [Fact]
        void Function_throws_when_return_type_not_int_or_double()
        {
            var name = new IdentifierToken("SendRequest", default);

            var scope = new SymbolScope();

            var ast = new FunctionAst(
                new KeywordToken(Keyword.Fn, default), name, new OperatorToken(Operator.LeftParen, default),
                functionParamList: null, new OperatorToken(Operator.RightParen, default),
                new OperatorToken(Operator.Arrow, default), new IdentifierToken("char", default),
                new Mock<BlockStatementAst>().Object
                );

            var generator = ConfigureGenerator(scope, mock =>
            {
            });

            Assert.Throws<SemanticException>(() => generator.ProcessFunction(ast));
        }

        [Fact]
        void Function_throws_when_any_param_type_not_int_or_double()
        {
            var name = new IdentifierToken("SendRequest", default);

            var scope = new SymbolScope();

            var param1 = new FunctionParamAst(null, new IdentifierToken("length", default),
                new OperatorToken(Operator.Colon, default), new IdentifierToken("int", default));
            var param2 = new FunctionParamAst(null, new IdentifierToken("ip", default),
                new OperatorToken(Operator.Colon, default), new IdentifierToken("string", default));
            var paramList = new FunctionParamListAst(new FunctionParamAst[] { param1, param2 });

            var ast = new FunctionAst(
                new KeywordToken(Keyword.Fn, default), name, new OperatorToken(Operator.LeftParen, default),
                paramList, new OperatorToken(Operator.RightParen, default),
                new OperatorToken(Operator.Arrow, default), returnType: new IdentifierToken("int", default),
                new Mock<BlockStatementAst>().Object
                );

            var generator = ConfigureGenerator(scope, mock =>
            {
            });

            Assert.Throws<SemanticException>(() => generator.ProcessFunction(ast));
        }

        [Fact]
        void Function_throws_when_any_param_name_exist_in_shallow()
        {
            var name = new IdentifierToken("SendRequest", default);

            var scope = new SymbolScope();

            scope.AddSymbol(new VariableSymbol("length1", false, false, DataType.Double));

            var param1 = new FunctionParamAst(null, new IdentifierToken("length1", default),
                new OperatorToken(Operator.Colon, default), new IdentifierToken("int", default));
            var param2 = new FunctionParamAst(null, new IdentifierToken("length2", default),
                new OperatorToken(Operator.Colon, default), new IdentifierToken("int", default));
            var paramList = new FunctionParamListAst(new FunctionParamAst[] { param1, param2 });

            var ast = new FunctionAst(
                new KeywordToken(Keyword.Fn, default), name, new OperatorToken(Operator.LeftParen, default),
                paramList, new OperatorToken(Operator.RightParen, default),
                new OperatorToken(Operator.Arrow, default), returnType: new IdentifierToken("int", default),
                new Mock<BlockStatementAst>().Object
                );

            var generator = ConfigureGenerator(scope, mock =>
            {
            });

            Assert.Throws<SemanticException>(() => generator.ProcessFunction(ast));
        }

        [Fact]
        void Function_throws_when_duplicated_param_name()
        {
            var name = new IdentifierToken("SendRequest", default);

            var scope = new SymbolScope();

            var param1 = new FunctionParamAst(null, new IdentifierToken("length1", default),
                new OperatorToken(Operator.Colon, default), new IdentifierToken("int", default));
            var param2 = new FunctionParamAst(null, new IdentifierToken("length1", default),
                new OperatorToken(Operator.Colon, default), new IdentifierToken("int", default));
            var paramList = new FunctionParamListAst(new FunctionParamAst[] { param1, param2 });

            var functionBlock = new Mock<BlockStatementAst>().Object;
            var ast = new FunctionAst(
                new KeywordToken(Keyword.Fn, default), name, new OperatorToken(Operator.LeftParen, default),
                paramList, new OperatorToken(Operator.RightParen, default),
                new OperatorToken(Operator.Arrow, default), returnType: new IdentifierToken("int", default),
                functionBlock
                );

            var generator = ConfigureGenerator(scope, mock =>
            {
                mock.Setup(p => p.ProcessBlockStatement(functionBlock, It.IsAny<bool>()));
            });

            Assert.Throws<SemanticException>(() => generator.ProcessFunction(ast));
        }

        [Fact]
        void Function_success_and_write_symbol_table()
        {
            var name = new IdentifierToken("SendRequest", default);

            var scope = new SymbolScope();
            scope.AddSymbol(new VariableSymbol("lll3", false, false, DataType.Long));

            var param1 = new FunctionParamAst(null, new IdentifierToken("length1", default),
                new OperatorToken(Operator.Colon, default), new IdentifierToken("int", default));
            var paramList = new FunctionParamListAst(new FunctionParamAst[] { param1 });

            var functionBlock = new Mock<BlockStatementAst>().Object;

            var ast = new FunctionAst(
                new KeywordToken(Keyword.Fn, default), name, new OperatorToken(Operator.LeftParen, default),
                paramList, new OperatorToken(Operator.RightParen, default),
                new OperatorToken(Operator.Arrow, default), returnType: new IdentifierToken("int", default),
                functionBlock
                );

            var generator = ConfigureGenerator(scope, mock =>
            {
                mock.Setup(p => p.ProcessBlockStatement(functionBlock, It.IsAny<bool>()));
            });

            generator.ProcessFunction(ast);

            // Assert
            Assert.True(scope.FindSymbolShallow(param1.Name.Value, out Symbol sym1));
            var var1 = Assert.IsType<VariableSymbol>(sym1);
            Assert.Equal(param1.IsConstant, var1.IsConstant);
            Assert.False(var1.IsGlobal);
            Assert.Equal(DataType.Long, var1.Type);
            Assert.Equal(param1.Name.Value, var1.Name);

        }
    }
}
