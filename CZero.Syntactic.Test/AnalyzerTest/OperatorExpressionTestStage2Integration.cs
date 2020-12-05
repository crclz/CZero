using Ardalis.GuardClauses;
using CZero.Lexical;
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
    public partial class OperatorExpressionTestStage2
    {
        public object CalculateExpression(string sourceCode)
        {
            Guard.Against.Null(sourceCode, nameof(sourceCode));
            var lexer = new Lexer(sourceCode);
            var tokens = lexer.Parse().ToList();
            var tokenReder = new TokenReader(tokens);

            var syntacticAnalyzer = Configure(new Mock<SyntacticAnalyzer>(tokenReder)).Object;

            var success = syntacticAnalyzer.TryExpression(out ExpressionAst expression);
            Assert.True(success);
            Assert.True(tokenReder.ReachedEnd, sourceCode);

            var value = expression.Calculate();
            return value;
        }

        private void AssertValue(string sourceCode, object expectedValue)
        {
            var value = CalculateExpression(sourceCode);

            Assert.True(expectedValue.Equals(value),
                $"Expected: {expectedValue}, Actual: {value}. SourceCode: {sourceCode}");
        }

        [Fact]
        void TestSamplesInStageOne()
        {
            foreach (var sample in OperatorExpressionTestStage1.SampleList1)
            {
                AssertValue(sample.Source, sample.Value);
            }

            foreach (var sample in OperatorExpressionTestStage1.SamplesWithNegative)
            {
                AssertValue(sample.Source, sample.Value);
            }
        }

        static int intsum(params int[] ints)
        {
            return ints.Sum();
        }

        const int i1 = 1, i2 = 2, i5 = 5;
        const double d1 = 1.0, d2 = 2.0, d5 = 5.0;

        internal static List<(string Source, object Value)> MySamples = new List<(string, object)>
        {
            ("intsum(-1)",intsum(-1)), ("intsum(-(-123)+i2)",intsum(-(-123)+i2)),
            ("-(-(-23141123)+i5)",-(-(-23141123)+i5)),("-(-0)",-(-0)),
            ("-1+1",-1+1),("-123132+i1+i1+i2+213123",-123132+i1+i1+i2+213123),
            ("-13124+i1/-6324+i5",-13124+i1/-6324+i5),("-0/-232323",-0/-232323),("-1/-32323232",-1/-32323232),

            ("0.0*123212.0*d2",0.0*123212.0*d2),("123.123*6666.5343*d5",123.123*6666.5343*d5),
            ("-12312.123E-4 * d2*-321343.1e+5",-12312.123E-4 * d2*-321343.1e+5),

            ("intsum(-321+323+0+2+323+323,i2,i5,i2,65)",intsum(-321+323+0+2+323+323,i2,i5,i2,65)),

            // 无括号
            ("-i1+2-3*4+i5/6*7+i2/9-8*1/3+6-323*7+888+6324/9/1-3/1/2-888/2*5",
            -i1+2-3*4+i5/6*7+i2/9-8*1/3+6-323*7+888+6324/9/1-3/1/2-888/2*5),

            // 多重sum
            ("-1+(((intsum(2,5,5,intsum(3,3),intsum(5),intsum())-3)*4)+5/6*7)+intsum(23/(9-8),(1))/(3+6-323*7)",
            -1+(((intsum(2,5,5,intsum(3,3),intsum(5),intsum())-3)*4)+5/6*7)+intsum(23/(9-8),(1))/(3+6-323*7)),
            ("intsum(6657,9418,intsum(1,2*3)+4)*55",intsum(6657,9418,intsum(1,2*3)+4)*55),

            // 比较
            ("intsum(1)>2",intsum(1)>2),("4>intsum(3)",4>intsum(3)),
            ("1+2*intsum(3,5,i2)*4*5>1*2*8889",1+2*intsum(3,5,i2)*4*5>1*2*8889)
        };

        [Fact]
        void TestIntegrationSamples()
        {
            foreach (var sample in MySamples)
            {
                AssertValue(sample.Source, sample.Value);
            }
        }
    }
}
