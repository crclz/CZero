﻿using CZero.Lexical.Tokens;
using CZero.Syntactic.Ast;
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
        static IntermediateCodeGenerator Configure(Action<Mock<IntermediateCodeGenerator>> action)
        {
            var mock = new Mock<IntermediateCodeGenerator>();
            mock.CallBase = true;

            action(mock);

            return mock.Object;
        }

        static IntermediateCodeGenerator MockStrongFactor(DataType type)
        {
            return Configure(mock =>
            {
                mock.Setup(p => p.ProcessStrongFactor(It.IsAny<StrongFactorAst>()))
                    .Returns(type);
            });
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

                    var generator = MockStrongFactor(type);

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

                var generator = MockStrongFactor(type);

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

                var generator = MockStrongFactor(type);

                Assert.Throws<SemanticException>(() => generator.ProcessGoodFactor(ast));
            }
        }


        static IEnumerable<(KeywordToken As, IdentifierToken Type)> RandomIntDoubleAsTypeList()
        {
            var l = new List<(KeywordToken As, IdentifierToken Type)>();
            var random = new Random();
            for (int i = 0; i < 8; i++)
            {
                var asToken = new KeywordToken(Keyword.As, default);
                if (random.Next(0, 100) < 50)
                    l.Add((asToken, new IdentifierToken("int", default)));
                else
                    l.Add((asToken, new IdentifierToken("double", default)));
            }

            return l;
        }

        static IntermediateCodeGenerator MockGoodFactor(DataType type)
        {
            return Configure(mock =>
            {
                mock.Setup(p => p.ProcessGoodFactor(It.IsAny<GoodFactorAst>()))
                    .Returns(type);
            });
        }

        [Fact]
        void ProcessFactor_returns_inner_type_when_no_as_type()
        {
            foreach (var innerType in new List<DataType> { DataType.Long, DataType.Double })
            {
                var emptyList = Enumerable.Empty<(KeywordToken, IdentifierToken)>();
                var ast = new FactorAst(new Mock<GoodFactorAst>().Object, emptyList);

                var generator = MockGoodFactor(innerType);

                var returningType = generator.ProcessFactor(ast);

                switch (innerType)
                {
                    case DataType.Long:
                        Assert.Equal(DataType.Long, returningType);
                        break;
                    case DataType.Double:
                        Assert.Equal(DataType.Double, returningType);
                        break;
                    default:
                        throw new Exception();
                }
            }
        }

        [Fact]
        void ProcessFactor_returns_last_astype_when_int_double_and_as_int_double()
        {
            for (int time = 0; time < 5; time++)
            {
                foreach (var asType in new List<DataType> { DataType.Long, DataType.Double })
                {
                    foreach (var innerType in new List<DataType> { DataType.Long, DataType.Double })
                    {
                        var asTypeList = RandomIntDoubleAsTypeList();
                        var typeDescriptor = asTypeList.Last().Type.Value;

                        var ast = new FactorAst(new Mock<GoodFactorAst>().Object, asTypeList);

                        var generator = MockGoodFactor(innerType);

                        var returningType = generator.ProcessFactor(ast);

                        switch (typeDescriptor)
                        {
                            case "int":
                                Assert.Equal(DataType.Long, returningType);
                                break;
                            case "double":
                                Assert.Equal(DataType.Double, returningType);
                                break;
                            default:
                                throw new Exception();
                        }
                    }
                }
            }

        }

        [Fact]
        void ProcessFactor_throws_when_any_astype_not_int_double()
        {
            foreach (var asType in Utils.GetEnumList<DataType>())
            {
                if (new[] { DataType.Long, DataType.Double }.Contains(asType))
                    continue;

                var asToken = new KeywordToken(Keyword.As, default);
                var asTypeList = new[] {
                    (asToken,new IdentifierToken("decimal",default)),
                    (asToken,new IdentifierToken("int",default)) };
                var ast = new FactorAst(new Mock<GoodFactorAst>().Object, asTypeList);

                var generator = MockGoodFactor(DataType.Long);
                Assert.Throws<SemanticException>(() => generator.ProcessFactor(ast));
            }
        }

        [Fact]
        void ProcessFactor_throws_when_innertype_not_int_double()
        {
            foreach (var innerType in Utils.GetEnumList<DataType>())
            {
                if (new[] { DataType.Long, DataType.Double }.Contains(innerType))
                    continue;

                var asToken = new KeywordToken(Keyword.As, default);
                var asTypeList = new[] {
                    (asToken,new IdentifierToken("double",default)) };
                var ast = new FactorAst(new Mock<GoodFactorAst>().Object, asTypeList);

                var generator = MockGoodFactor(innerType);
                Assert.Throws<SemanticException>(() => generator.ProcessFactor(ast));
            }
        }


        static IntermediateCodeGenerator MockFactor(DataType type)
        {
            return Configure(mock =>
            {
                mock.Setup(p => p.ProcessFactor(It.IsAny<FactorAst>()))
                    .Returns(type);
            });
        }

        [Fact]
        void ProcessTerm_throws_when_inner_not_int_or_double()
        {
            foreach (var innerType in Utils.GetEnumList<DataType>())
            {
                if (new[] { DataType.Long, DataType.Double }.Contains(innerType))
                    continue;

                var opToken = new OperatorToken(Operator.Mult, default);
                var opFactors = new[] {
                    (opToken, new Mock<FactorAst>().Object) };
                var ast = new TermAst(new Mock<FactorAst>().Object, opFactors);

                var generator = MockFactor(innerType);
                Assert.Throws<SemanticException>(() => generator.ProcessTerm(ast));
            }
        }

        [Fact]
        void ProcessTerm_returns_inner_type_when_list_empty()
        {
            foreach (var innerType in new[] { DataType.Long, DataType.Double })
            {
                var emptyList = Enumerable.Empty<(OperatorToken, FactorAst)>();

                var ast = new TermAst(new Mock<FactorAst>().Object, emptyList);

                var generator = MockFactor(innerType);

                Assert.Equal(innerType, generator.ProcessTerm(ast));
            }
        }

        [Fact]
        void ProcessTerm_throws_when_any_thing_in_list_not_first_type()
        {
            foreach (var innerType in new[] { DataType.Long, DataType.Double })
            {
                var opToken = new OperatorToken(Operator.Mult, default);

                var fac2 = new Mock<FactorAst>().Object;
                var fac3 = new Mock<FactorAst>().Object;

                var opFactors = new[] {
                    (opToken, fac2),
                    (opToken, fac3)
                };

                var firstFactor = new Mock<FactorAst>().Object;
                var ast = new TermAst(firstFactor, opFactors);

                var generator = Configure(mock =>
                {
                    mock.Setup(p => p.ProcessFactor(firstFactor)).Returns(DataType.Long);
                    mock.Setup(p => p.ProcessFactor(fac2)).Returns(DataType.Double);
                    mock.Setup(p => p.ProcessFactor(fac3)).Returns(DataType.Long);
                });

                Assert.Throws<SemanticException>(() => generator.ProcessTerm(ast));
            }
        }

        [Fact]
        void ProcessTerm_returns_the_type_when_list_is_ok()
        {
            foreach (var innerType in new[] { DataType.Long, DataType.Double })
            {
                var opToken = new OperatorToken(Operator.Mult, default);

                var fac2 = new Mock<FactorAst>().Object;
                var fac3 = new Mock<FactorAst>().Object;

                var opFactors = new[] {
                    (opToken, fac2),
                    (opToken, fac3)
                };

                var firstFactor = new Mock<FactorAst>().Object;
                var ast = new TermAst(firstFactor, opFactors);

                var generator = Configure(mock =>
                {
                    mock.Setup(p => p.ProcessFactor(firstFactor)).Returns(DataType.Double);
                    mock.Setup(p => p.ProcessFactor(fac2)).Returns(DataType.Double);
                    mock.Setup(p => p.ProcessFactor(fac3)).Returns(DataType.Double);
                });

                Assert.Equal(DataType.Double, generator.ProcessTerm(ast));
            }

        }

    }
}