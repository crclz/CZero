using Ardalis.GuardClauses;
using CZero.Syntactic.Ast.Expressions.OperatorExpression;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public virtual DataType ProcessTerm(TermAst term)
        {
            var firstType = ProcessFactor(term.Factor);
            if (!DataTypeHelper.IsLongOrDouble(firstType))
                throw new SemanticException($"First factor type {firstType} is not int or double");

            if (term.OpFactors.Count == 0)
                return firstType;

            foreach (var (op, factor) in term.OpFactors)
            {
                Debug.Assert(op.Value == Lexical.Tokens.Operator.Mult
                    || op.Value == Lexical.Tokens.Operator.Divide);

                var factorType = ProcessFactor(factor);
                if (factorType != firstType)
                    throw new SemanticException($"Factor type '{factorType}' should equal to first type '{firstType}'");
            }

            // list check ok
            return firstType;
        }

        public virtual DataType ProcessWeakTerm(WeakTermAst weak)
        {
            // Remember to assert each op is +|-
            var firstType = ProcessTerm(weak.Term);

            if (!DataTypeHelper.IsLongOrDouble(firstType))
                throw new SemanticException($"first type '{firstType}' should be int or double");

            if (weak.OpTerms.Count == 0)
                return firstType;

            foreach (var (op, term) in weak.OpTerms)
            {
                Debug.Assert(op.Value == Lexical.Tokens.Operator.Plus
                    || op.Value == Lexical.Tokens.Operator.Minus);

                var termType = ProcessTerm(term);
                if (termType != firstType)
                    throw new SemanticException(
                        $"Term type '{termType}' should be equal to first type '{firstType}'");

            }

            // list check ok
            return firstType;
        }
    }
}
