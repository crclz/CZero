using CZero.Lexical.Tokens;
using CZero.Syntactic.Ast.Functions;
using CZero.Syntactic.Ast.Statements;
using CZero.Syntactic.Ast.Statements.Declarative;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace CZero.Syntactic.Test.AnalyzerTest
{
    public class FunctionTest
    {
        [Fact]
        void FunctionParam_with_const_success()
        {
            // 'const'? IDENT ':' ty

            // Arrange
            var tokens = new Token[]
            {
                new KeywordToken(Keyword.Const, default),
                new IdentifierToken("size",default),
                new OperatorToken(Operator.Colon,default),
                new IdentifierToken("int",default),
            };
            var reader = new TokenReader(tokens);
            var analyzer = new SyntacticAnalyzer(reader);

            // Act
            var success = analyzer.TryFunctionParam(out FunctionParamAst parameter);

            // Assert
            Assert.True(success);
            Assert.NotNull(parameter);

            Assert.True(reader.ReachedEnd);

            Assert.True(parameter.IsConstant);
            Assert.Equal(tokens[0], parameter.Const);
            Assert.Equal(tokens[1], parameter.Name);
            Assert.Equal(tokens[2], parameter.Colon);
            Assert.Equal(tokens[3], parameter.Type);
        }

        [Fact]
        void FunctionParam_without_const_success()
        {
            // 'const'? IDENT ':' ty

            // Arrange
            var tokens = new Token[]
            {
                new IdentifierToken("size",default),
                new OperatorToken(Operator.Colon,default),
                new IdentifierToken("int",default),
            };
            var reader = new TokenReader(tokens);
            var analyzer = new SyntacticAnalyzer(reader);

            // Act
            var success = analyzer.TryFunctionParam(out FunctionParamAst parameter);

            // Assert
            Assert.True(success);
            Assert.NotNull(parameter);

            Assert.True(reader.ReachedEnd);

            Assert.False(parameter.IsConstant);

            Assert.Null(parameter.Const);
            Assert.Equal(tokens[0], parameter.Name);
            Assert.Equal(tokens[1], parameter.Colon);
            Assert.Equal(tokens[2], parameter.Type);
        }

        [Fact]
        void FunctionParamList_success_and_0()
        {
            // function_param (',' function_param)*

            for (int paramCount = 0; paramCount < 8; paramCount++)
            {
                // Arrange

                var tokens = new List<Token>();

                for (var i = 0; i < paramCount; i++)
                {
                    tokens.Add(new IdentifierToken("size" + i, default));
                    tokens.Add(new OperatorToken(Operator.Colon, default));
                    tokens.Add(new IdentifierToken("int", default));

                    tokens.Add(new OperatorToken(Operator.Comma, default));
                };
                if (paramCount > 0)
                    tokens.RemoveAt(tokens.Count - 1);// remove ','

                var reader = new TokenReader(tokens);
                var analyzer = new SyntacticAnalyzer(reader);

                // Act
                var success = analyzer.TryFunctionParamList(out FunctionParamListAst paramList);

                // Assert
                if (paramCount == 0)
                {
                    Assert.False(success);
                    Assert.Null(paramList);
                    Assert.Equal(0, reader._cursor);
                }
                else
                {
                    Assert.True(success);
                    Assert.NotNull(paramList);

                    Assert.True(reader.ReachedEnd);

                    Assert.Equal(paramCount, paramList.FunctionParams.Count);
                }
            }
        }

        [Fact]
        void FunctionAnalyzeTest()
        {
            // 'fn' IDENT '(' function_param_list? ')' '->' ty block_stmt

            for (int paramCount = 0; paramCount < 8; paramCount++)
            {
                // Arrange

                var tokens = new List<Token>()
                {
                    new KeywordToken(Keyword.Fn, default),
                    new IdentifierToken("print", default),
                    new OperatorToken(Operator.LeftParen, default)
                };

                var endingTokens = new Token[]
                {
                    new OperatorToken(Operator.RightParen, default),
                    new OperatorToken(Operator.Arrow, default),
                    new IdentifierToken("int", default),
                    new OperatorToken(Operator.LeftBrace, default),
                    new OperatorToken(Operator.RightBrace, default)
                };

                for (var i = 0; i < paramCount; i++)
                {
                    tokens.Add(new IdentifierToken("size" + i, default));
                    tokens.Add(new OperatorToken(Operator.Colon, default));
                    tokens.Add(new IdentifierToken("int", default));

                    tokens.Add(new OperatorToken(Operator.Comma, default));
                };
                if (paramCount > 0)
                    tokens.RemoveAt(tokens.Count - 1);// remove ','

                tokens.AddRange(endingTokens);

                var reader = new TokenReader(tokens);
                var analyzer = new SyntacticAnalyzer(reader);

                // Act
                var success = analyzer.TryFunction(out FunctionAst function);

                // Assert
                Assert.True(success);
                Assert.NotNull(function);
                Assert.True(reader.ReachedEnd);

                Assert.Equal(tokens[0], function.Fn);
                Assert.Equal(tokens[1], function.Name);
                Assert.Equal(tokens[2], function.LeftParen);

                Assert.Equal(endingTokens[0], function.RightParen);
                Assert.Equal(endingTokens[1], function.Arrow);
                Assert.Equal(endingTokens[2], function.ReturnType);

                // params
                if (paramCount == 0)
                {
                    Assert.False(function.HasParams);
                }
                else
                {
                    Assert.Equal(paramCount, function.FunctionParamList.FunctionParams.Count);
                }
            }
        }
    }
}
