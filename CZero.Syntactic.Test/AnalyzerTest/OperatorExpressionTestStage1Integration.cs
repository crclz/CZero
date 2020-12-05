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
    public partial class OperatorExpressionTestStage1
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
        void IntegrationTestIntValuesOfNotNegative()
        {
            var random = new Random(90001);

            for (int i = 0; i < 1000; i++)
            {
                var x = random.Next(0, int.MaxValue);
                AssertValue(x.ToString(), x);
            }
        }

        internal static List<(string Source, object Value)> SampleList1 = new List<(string, object)>
        {
            ("1",1), ("123",123), ("23141123",23141123),("0",0),
            ("1+1",1+1),("123132+213123",123132+213123),("8882331+2813812",8882331+2813812),
            ("0-2313121",0-2313121),("8848-8849",8848-8849),("1231231-0",1231231-0),
            ("0*123212",0*123212), ("123*6666",123*6666), ("312*7146",312*7146),
            ("13124/6324",13124/6324),("0/232323",0/232323),("1/32323232",1/32323232),

            ("0.0*123212.0",0.0*123212.0),("123.123*6666.5343",123.123*6666.5343),
            ("12312.123E-4 * 321343.1e+5",12312.123E-4 * 321343.1e+5),

            ("321+323+0+2+323+323",321+323+0+2+323+323),
            ("321-232-3232+323-323",321-232-3232+323-323),

            // 无括号
            ("1+2-3*4+5/6*7+23/9-8*1/3+6-323*7+888+6324/9/1-3/1/2-888/2*5",
            1+2-3*4+5/6*7+23/9-8*1/3+6-323*7+888+6324/9/1-3/1/2-888/2*5),

            ("1.0+2.23-3.45*4.23+5.31/6.312*7.43+23.0/9.44-8.123*1.41/3.14+6.553-323.5-888.3/2.2*5.6",
            1.0+2.23-3.45*4.23+5.31/6.312*7.43+23.0/9.44-8.123*1.41/3.14+6.553-323.5-888.3/2.2*5.6),

            // 多重括号
            ("1+(((2-3)*4)+5/6*7)+23/(9-8)*(1)/(3+6-323*7)+((888+6324/9))/(((1-3)/1/2)-888/2*5)",
            1+(((2-3)*4)+5/6*7)+23/(9-8)*(1)/(3+6-323*7)+((888+6324/9))/(((1-3)/1/2)-888/2*5)),
        };

        [Fact]
        void IntegrationSampleTests()
        {
            foreach (var sample in SampleList1)
            {
                AssertValue(sample.Source, sample.Value);
            }
        }


        internal static List<(string Source, object Value)> SamplesWithNegative = new List<(string, object)>
        {
            ("-1",-1), ("-(-123)",-(-123)), ("-(-(-23141123))",-(-(-23141123))),("-(-0)",-(-0)),
            ("-1+1",-1+1),("-123132+213123",-123132+213123),("-8882331+2813812",-8882331+2813812),
            ("0-2313121",0-2313121),("-8848-8849",-8848-8849),("-1231231-0",-1231231-0),
            ("-0*123212",-0*-123212), ("-123*-6666",-123*-6666), ("-312*-7146",-312*-7146),
            ("-13124/-6324",-13124/-6324),("-0/-232323",-0/-232323),("-1/-32323232",-1/-32323232),

            ("0.0*123212.0",0.0*123212.0),("123.123*6666.5343",123.123*6666.5343),
            ("-12312.123E-4 * -321343.1e+5",-12312.123E-4 * -321343.1e+5),

            ("-321+323+0+2+323+323",-321+323+0+2+323+323),
            ("-321-232-3232+323-323",-321-232-3232+323-323),

            // 无括号
            ("-1+2-3*4+5/6*7+23/9-8*1/3+6-323*7+888+6324/9/1-3/1/2-888/2*5",
            -1+2-3*4+5/6*7+23/9-8*1/3+6-323*7+888+6324/9/1-3/1/2-888/2*5),

            ("-1.0+2.23-3.45*4.23+5.31/6.312*7.43+23.0/9.44-8.123*1.41/3.14+6.553-323.5-888.3/2.2*5.6",
            -1.0+2.23-3.45*4.23+5.31/6.312*7.43+23.0/9.44-8.123*1.41/3.14+6.553-323.5-888.3/2.2*5.6),

            // 多重括号
            ("-1+(((2-3)*4)+5/6*7)+23/(9-8)*(1)/(3+6-323*7)+((888+6324/9))/(((1-3)/1/2)-888/2*5)",
            -1+(((2-3)*4)+5/6*7)+23/(9-8)*(1)/(3+6-323*7)+((888+6324/9))/(((1-3)/1/2)-888/2*5)),
        };

        [Fact]
        void IntegrationSampleWithNegativeTests()
        {
            foreach (var sample in SamplesWithNegative)
            {
                AssertValue(sample.Source, sample.Value);
            }
        }
    }
}
