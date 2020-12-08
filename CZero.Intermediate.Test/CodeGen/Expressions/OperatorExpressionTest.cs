using CZero.Intermediate.Symbols;
using CZero.Lexical;
using CZero.Syntactic;
using CZero.Syntactic.Ast;
using CZero.Syntactic.Ast.Expressions.OperatorExpression;
using CZero.Syntactic.Test.AnalyzerTest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Xunit;
using Xunit.Abstractions;

namespace CZero.Intermediate.Test.CodeGen.Expressions
{
    public class OperatorExpressionTest
    {
        public static List<object[]> Generate(string sourceCode)
        {
            var lexer = new Lexer(sourceCode);
            var tokens = lexer.Parse().ToList();

            var tokenReader = new TokenReader(tokens);
            var syntacticAnalyzer = new SyntacticAnalyzer(tokenReader);

            var success = syntacticAnalyzer.TryOperatorExpression(out OperatorExpressionAst ast);
            Assert.True(success);
            Assert.True(tokenReader.ReachedEnd);

            var rootScope = new SymbolScope();
            var generator = new IntermediateCodeGenerator(rootScope);
            generator.CodeGenerationEnabled = true;
            generator.ReturnCheckEnabled = true;

            generator.ProcessOperatorExpression(ast);

            return generator.Bucket.Pop();
        }

        [Fact]
        void TestExpressionSamples()
        {
            var samples = new List<(string, object)>();
            samples.AddRange(OperatorExpressionTestStage1.SampleList1);
            samples.AddRange(OperatorExpressionTestStage1.SamplesWithNegative);

            foreach (var (src, r) in samples)
            {
                var instructions = Generate(src);

                var nvam = new Nvam(instructions);

                while (!nvam.ReachedEnd)
                {
                    nvam.Next();
                }

                Assert.Single(nvam.StackView);
                var actualResult = nvam.StackView[0];

                // Normalize expected result
                var expectedResult = r;
                if (expectedResult is bool boolResult)
                {
                    if (boolResult == true)
                        expectedResult = 1L;
                    else
                        expectedResult = 0L;
                }
                if (expectedResult is int intResult)
                {
                    expectedResult = (long)intResult;
                }

                Assert.Equal(expectedResult, actualResult);
            }
        }
    }
}
