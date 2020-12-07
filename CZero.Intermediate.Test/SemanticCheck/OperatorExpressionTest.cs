using CZero.Lexical.Tokens;
using CZero.Syntactic.Ast.Expressions.OperatorExpression;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace CZero.Intermediate.Test
{
    public class OperatorExpressionTest
    {
        static IntermediateCodeGenerator MockProcessStrongFactor(DataType type)
        {
            var mock = new Mock<IntermediateCodeGenerator>();
            mock.CallBase = true;
            mock.Setup(p => p.ProcessStrongFactor(It.IsAny<StrongFactorAst>())).Returns(type);
            return mock.Object;
        }

        [Fact]
        void ProcessGoodFactor_return_type_whenever_inner_type_is_double_or_long()
        {
            foreach (var type in new List<DataType> { DataType.Long, DataType.Double })
            {
                foreach (var isNegative in new List<bool> { true, false })
                {
                    var negativeOperators = new List<OperatorToken>();
                    if (isNegative)
                        negativeOperators.Add(new OperatorToken(Operator.Minus, default));

                    var ast = new GoodFactorAst(negativeOperators, new Mock<StrongFactorAst>().Object);

                    var generator = MockProcessStrongFactor(type);

                    var returningType = generator.ProcessGoodFactor(ast);
                    Assert.Equal(type, returningType);
                }
            }
        }

        [Fact]
        void ProcessGoodFactor_return_type_when_not_negative()
        {
            foreach (var type in Utils.GetEnumList<DataType>())
            {
                var negatives = Enumerable.Empty<OperatorToken>();// not negative
                var ast = new GoodFactorAst(negatives, new Mock<StrongFactorAst>().Object);

                var generator = MockProcessStrongFactor(type);

                var returningType = generator.ProcessGoodFactor(ast);
                Assert.Equal(type, returningType);
            }
        }


        [Fact]
        void ProcessGoodFactor_throws_when_negative_and_inner_not_int_double()
        {
            foreach (var type in Utils.GetEnumList<DataType>())
            {
                if (DataTypeHelper.IsLongOrDouble(type))
                    continue;

                var negatives = new OperatorToken[] { new OperatorToken(Operator.Minus, default) };
                var ast = new GoodFactorAst(negatives, new Mock<StrongFactorAst>().Object);

                var generator = MockProcessStrongFactor(type);

                Assert.Throws<SemanticException>(() => generator.ProcessGoodFactor(ast));
            }
        }
    }

}