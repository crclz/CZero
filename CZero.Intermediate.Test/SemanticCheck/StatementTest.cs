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

namespace CZero.Intermediate.Test.SemanticCheck
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

            return mock.Object;
        }


        #region ProcessLetDeclarationStatement

        [Fact]
        void ProcessLetDeclaration_throws_when_symbol_exist_in_shallow()
        {
            var existingSymbol = new VariableSymbol("webClient", false, false, DataType.Long);
            var scope = new SymbolScope();
            scope.AddSymbol(existingSymbol);

            var name = new IdentifierToken("webClient", default);

            var ast = new LetDeclarationStatementAst(
                new KeywordToken(Keyword.Let, default), name, new OperatorToken(Operator.Colon, default),
                new IdentifierToken("int", default), new OperatorToken(Operator.Assign, default),
                initialExpression: null, new OperatorToken(Operator.Semicolon, default));

            var generator = ConfigureGenerator(scope, mock =>
            {
            });

            Assert.Throws<SemanticException>(() => generator.ProcessLetDeclarationStatement(ast));
        }

        [Fact]
        void ProcessLetDeclaration_throws_when_type_is_not_int_or_double()
        {
            var scope = new SymbolScope();

            var name = new IdentifierToken("requestsClient", default);

            var ast = new LetDeclarationStatementAst(
                new KeywordToken(Keyword.Let, default), name, new OperatorToken(Operator.Colon, default),
                /*here*/ type: new IdentifierToken("char", default),
                new OperatorToken(Operator.Assign, default),
                initialExpression: null, new OperatorToken(Operator.Semicolon, default));

            var generator = ConfigureGenerator(scope, mock =>
            {
            });

            Assert.Throws<SemanticException>(() => generator.ProcessLetDeclarationStatement(ast));
        }

        [Fact]
        void ProcessLetDeclaration_throws_if_expression_type_not_match()
        {
            var scope = new SymbolScope();

            var name = new IdentifierToken("requestsClient", default);

            var initialExpression = new Mock<ExpressionAst>().Object;

            var initialType = DataType.Long;
            var declareType = "double";

            var ast = new LetDeclarationStatementAst(
                new KeywordToken(Keyword.Let, default), name, new OperatorToken(Operator.Colon, default),
                type: new IdentifierToken(declareType, default),
                new OperatorToken(Operator.Assign, default),
                initialExpression, new OperatorToken(Operator.Semicolon, default));

            var generator = ConfigureGenerator(scope, mock =>
            {
                mock.Setup(p => p.ProcessExpression(initialExpression)).Returns(initialType);
            });

            Assert.Throws<SemanticException>(() => generator.ProcessLetDeclarationStatement(ast));
        }

        [Fact]
        void ProcessLetDeclaration_adds_symbol_if_previous_shits_ok_and_no_initial_expr()
        {
            var scope = new SymbolScope();

            var name = new IdentifierToken("requestsClient", default);

            var declareType = "double";

            var ast = new LetDeclarationStatementAst(
                new KeywordToken(Keyword.Let, default), name, new OperatorToken(Operator.Colon, default),
                type: new IdentifierToken(declareType, default),
                new OperatorToken(Operator.Assign, default),
                initialExpression: null, new OperatorToken(Operator.Semicolon, default));

            var generator = ConfigureGenerator(scope, mock =>
            {
            });

            generator.ProcessLetDeclarationStatement(ast);

            Assert.True(scope.FindSymbolShallow(name.Value, out Symbol symbol));
            var variableSymbol = Assert.IsType<VariableSymbol>(symbol);

            Assert.Equal(name.Value, variableSymbol.Name);
            Assert.False(variableSymbol.IsConstant);
            Assert.True(variableSymbol.IsGlobal);
            Assert.Equal(DataType.Double, variableSymbol.Type);
        }

        [Fact]
        void ProcessLetDeclaration_adds_symbol_if_all_ok()
        {
            var scope = new SymbolScope();

            var name = new IdentifierToken("requestsClient", default);

            var initialExpression = new Mock<ExpressionAst>().Object;

            var initialType = DataType.Double;
            var declareType = "double";

            var ast = new LetDeclarationStatementAst(
                new KeywordToken(Keyword.Let, default), name, new OperatorToken(Operator.Colon, default),
                type: new IdentifierToken(declareType, default),
                new OperatorToken(Operator.Assign, default),
                initialExpression, new OperatorToken(Operator.Semicolon, default));

            var generator = ConfigureGenerator(scope, mock =>
            {
                mock.Setup(p => p.ProcessExpression(initialExpression)).Returns(initialType);
            });

            generator.ProcessLetDeclarationStatement(ast);

            Assert.True(scope.FindSymbolShallow(name.Value, out Symbol symbol));
            var variableSymbol = Assert.IsType<VariableSymbol>(symbol);

            Assert.Equal(name.Value, variableSymbol.Name);
            Assert.False(variableSymbol.IsConstant);
            Assert.True(variableSymbol.IsGlobal);
            Assert.Equal(DataType.Double, variableSymbol.Type);
        }

        #endregion


        #region ProcessConstDeclaration
        /* const_decl_stmt -> 'const' IDENT ':' ty '=' expr ';'
         * 
         * ProcessConstDeclaration_adds_symbol_if_all_ok
         */

        [Fact]
        void ProcessConstDeclaration_throws_when_symbol_exist_in_shallow()
        {
            var existingSymbol = new VariableSymbol("pi", false, false, DataType.Long);
            var scope = new SymbolScope();
            scope.AddSymbol(existingSymbol);

            var name = new IdentifierToken("pi", default);

            var valueExpression = new Mock<ExpressionAst>().Object;

            var ast = new ConstDeclarationStatementAst(
                new KeywordToken(Keyword.Const, default), name, new OperatorToken(Operator.Colon, default),
                new IdentifierToken("int", default), new OperatorToken(Operator.Assign, default),
                valueExpression, new OperatorToken(Operator.Semicolon, default));

            var generator = ConfigureGenerator(scope, mock =>
            {
            });

            Assert.Throws<SemanticException>(() => generator.ProcessConstDeclarationStatement(ast));
        }

        [Fact]
        void ProcessConstDeclaration_throws_when_type_is_not_int_or_double()
        {
            var scope = new SymbolScope();

            var name = new IdentifierToken("pi", default);

            var ast = new ConstDeclarationStatementAst(
                new KeywordToken(Keyword.Const, default), name, new OperatorToken(Operator.Colon, default),
                /*here*/ type: new IdentifierToken("void", default),
                new OperatorToken(Operator.Assign, default),
                new Mock<ExpressionAst>().Object, new OperatorToken(Operator.Semicolon, default));

            var generator = ConfigureGenerator(scope, mock =>
            {
            });

            Assert.Throws<SemanticException>(() => generator.ProcessConstDeclarationStatement(ast));
        }

        [Fact]
        void ProcessConstDeclaration_throws_if_expression_type_not_match()
        {
            var scope = new SymbolScope();

            var name = new IdentifierToken("pi", default);

            var initialExpression = new Mock<ExpressionAst>().Object;

            var initialType = DataType.Long;
            var declareType = "double";

            var ast = new ConstDeclarationStatementAst(
                new KeywordToken(Keyword.Const, default), name, new OperatorToken(Operator.Colon, default),
                type: new IdentifierToken(declareType, default),
                new OperatorToken(Operator.Assign, default),
                initialExpression, new OperatorToken(Operator.Semicolon, default));

            var generator = ConfigureGenerator(scope, mock =>
            {
                mock.Setup(p => p.ProcessExpression(initialExpression)).Returns(initialType);
            });

            Assert.Throws<SemanticException>(() => generator.ProcessConstDeclarationStatement(ast));
        }

        [Fact]
        void ProcessConstDeclaration_adds_symbol_if_all_ok()
        {
            var scope = new SymbolScope();

            var name = new IdentifierToken("pi", default);

            var initialExpression = new Mock<ExpressionAst>().Object;

            var initialType = DataType.Double;
            var declareType = "double";

            var ast = new ConstDeclarationStatementAst(
                new KeywordToken(Keyword.Const, default), name, new OperatorToken(Operator.Colon, default),
                type: new IdentifierToken(declareType, default),
                new OperatorToken(Operator.Assign, default),
                initialExpression, new OperatorToken(Operator.Semicolon, default));

            var generator = ConfigureGenerator(scope, mock =>
            {
                mock.Setup(p => p.ProcessExpression(initialExpression)).Returns(initialType);
            });

            generator.ProcessConstDeclarationStatement(ast);

            Assert.True(scope.FindSymbolShallow(name.Value, out Symbol symbol));
            var variableSymbol = Assert.IsType<VariableSymbol>(symbol);

            Assert.Equal(name.Value, variableSymbol.Name);
            Assert.True(variableSymbol.IsConstant);
            Assert.True(variableSymbol.IsGlobal);
            Assert.Equal(DataType.Double, variableSymbol.Type);
        }

        #endregion


        #region If
        [Fact]
        void ProcessIfStatement_throws_when_condition_not_bool()
        {
            var scope = new SymbolScope();
            var condition = new Mock<ExpressionAst>().Object;

            var ast = new IfStatementAst(
                new KeywordToken(Keyword.If, default), condition, new Mock<BlockStatementAst>().Object,
                @else: null, null, null);

            var generator = ConfigureGenerator(scope, mock =>
            {
                mock.Setup(p => p.ProcessExpression(condition)).Returns(DataType.Long);
            });

            Assert.Throws<SemanticException>(() => generator.ProcessIfStatement(ast));
        }

        [Fact]
        void ProcessIfStatement_ok_when_condition_is_bool()
        {
            var scope = new SymbolScope();
            var condition = new Mock<ExpressionAst>().Object;

            var ast = new IfStatementAst(
                new KeywordToken(Keyword.If, default), condition, new Mock<BlockStatementAst>().Object,
                @else: null, null, null);

            var generator = ConfigureGenerator(scope, mock =>
            {
                mock.Setup(p => p.ProcessExpression(condition)).Returns(DataType.Bool);
            });

            generator.ProcessIfStatement(ast);
        }
        #endregion
    }
}
