using Ardalis.GuardClauses;
using CZero.Syntactic.Ast.Expressions.OperatorExpression;
using System;
using System.Collections.Generic;
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
    }
}
