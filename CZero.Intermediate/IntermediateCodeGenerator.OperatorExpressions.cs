using Ardalis.GuardClauses;
using CZero.Syntactic.Ast.Expressions.OperatorExpression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CZero.Intermediate
{
    partial class IntermediateCodeGenerator
    {
        public virtual DataType ProcessStrongFactor(StrongFactorAst strongFactor)
        {
            Guard.Against.Null(strongFactor, nameof(strongFactor));

            throw new NotImplementedException();
        }

        public virtual DataType ProcessGoodFactor(GoodFactorAst goodFactor)
        {
            Guard.Against.Null(goodFactor, nameof(goodFactor));
            // { - } strong_factor
            // 检查能否取符号

            var strongFactorType = ProcessStrongFactor(goodFactor.StrongFactor);

            if (goodFactor.IsNegative && !DataTypeHelper.IsLongOrDouble(strongFactorType))
                throw new SemanticException($"{strongFactorType} cannot be negative");

            return strongFactorType;
        }

        public virtual DataType ProcessFactor(FactorAst factor)
        {
            var type = ProcessGoodFactor(factor.GoodFactor);

            if (!DataTypeHelper.IsLongOrDouble(type))
            {
                throw new SemanticException($"Inner type '{type}' is not double or int");
            }

            if (factor.AsTypeList.Count == 0)
                return type;

            foreach (var a in factor.AsTypeList)
            {
                switch (a.TypeToken.Value)
                {
                    case "int":
                        type = DataType.Long;
                        break;
                    case "double":
                        type = DataType.Double;
                        break;
                    default:
                        throw new SemanticException($"Wrong type cast: {a.TypeToken.Value}");
                }
            }

            return type;
        }
    }
}
