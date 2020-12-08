using Ardalis.GuardClauses;
using CZero.Lexical.Tokens;
using CZero.Syntactic.Ast.Expressions;
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

            // assign_expr | call_expr | literal_expr | ident_expr | group_expr
            var expr = strongFactor.SingleExpression;
            if (expr is AssignExpressionAst assignExpression)
                return ProcessAssignExpression(assignExpression);
            else if (expr is CallExpressionAst callExpression)
                return ProcessCallExpression(callExpression);
            else if (expr is LiteralExpressionAst literalExpression)
                return ProcessLiteralExpression(literalExpression);
            else if (expr is IdentExpressionAst identExpression)
                return ProcessIdentExpression(identExpression);
            else if (expr is GroupExpressionAst groupExpression)
                return ProcessGroupExpression(groupExpression);
            else
                throw new ArgumentException($"Unsupported type: {expr.GetType()}");
        }

        public virtual DataType ProcessGoodFactor(GoodFactorAst goodFactor)
        {
            Guard.Against.Null(goodFactor, nameof(goodFactor));
            // { - } strong_factor
            // 检查能否取符号

            var strongFactorType = ProcessStrongFactor(goodFactor.StrongFactor);

            if (goodFactor.IsNegative && !DataTypeHelper.IsLongOrDouble(strongFactorType))
                throw new SemanticException($"{strongFactorType} cannot be negative");

            if (CodeGenerationEnabled)
            {
                if (goodFactor.IsNegative)
                {
                    Debug.Assert(DataTypeHelper.IsLongOrDouble(strongFactorType));
                    var negativeOperation = "neg." + DataTypeHelper.Suffix(strongFactorType);
                    Bucket.Add(negativeOperation);
                }
            }

            return strongFactorType;
        }

        public virtual DataType ProcessFactor(FactorAst factor)
        {
            var firstType = ProcessGoodFactor(factor.GoodFactor);

            if (factor.AsTypeList.Count == 0)
                return firstType;

            if (!DataTypeHelper.IsLongOrDouble(firstType))
            {
                throw new SemanticException($"First type '{firstType}' is not double or int");
            }

            var type = firstType;

            foreach (var a in factor.AsTypeList)
            {
                DataType t2;
                switch (a.TypeToken.Value)
                {
                    case "int":
                        t2 = DataType.Long;
                        break;
                    case "double":
                        t2 = DataType.Double;
                        break;
                    default:
                        throw new SemanticException($"Wrong type cast: {a.TypeToken.Value}");
                }

                if (CodeGenerationEnabled)
                {
                    var src = DataTypeHelper.Suffix(type);
                    var dst = DataTypeHelper.Suffix(t2);

                    Bucket.Add(src + "to" + dst);
                }

                type = t2;
            }

            return type;
        }

        public virtual DataType ProcessTerm(TermAst term)
        {
            var firstType = ProcessFactor(term.Factor);

            if (term.OpFactors.Count == 0)
                return firstType;

            if (!DataTypeHelper.IsLongOrDouble(firstType))
                throw new SemanticException($"First factor type {firstType} is not int or double");

            char suffix = DataTypeHelper.Suffix(firstType);

            foreach (var (op, factor) in term.OpFactors)
            {
                Debug.Assert(op.Value == Lexical.Tokens.Operator.Mult
                    || op.Value == Lexical.Tokens.Operator.Divide);

                var factorType = ProcessFactor(factor);
                if (factorType != firstType)
                    throw new SemanticException($"Factor type '{factorType}' should equal to first type '{firstType}'");

                if (CodeGenerationEnabled)
                {
                    if (op.Value == Lexical.Tokens.Operator.Mult)
                        Bucket.Add("mul." + suffix);
                    else
                        Bucket.Add("div." + suffix);
                }
            }

            // list check ok
            return firstType;
        }

        public virtual DataType ProcessWeakTerm(WeakTermAst weak)
        {
            // Remember to assert each op is +|-
            var firstType = ProcessTerm(weak.Term);

            if (weak.OpTerms.Count == 0)
                return firstType;

            if (!DataTypeHelper.IsLongOrDouble(firstType))
                throw new SemanticException($"first type '{firstType}' should be int or double");

            char suffix = DataTypeHelper.Suffix(firstType);

            foreach (var (op, term) in weak.OpTerms)
            {
                Debug.Assert(op.Value == Lexical.Tokens.Operator.Plus
                    || op.Value == Lexical.Tokens.Operator.Minus);

                var termType = ProcessTerm(term);
                if (termType != firstType)
                    throw new SemanticException(
                        $"Term type '{termType}' should be equal to first type '{firstType}'");

                if (CodeGenerationEnabled)
                {
                    if (op.Value == Lexical.Tokens.Operator.Plus)
                        Bucket.Add("add." + suffix);
                    else
                        Bucket.Add("sub." + suffix);
                }
            }

            // list check ok
            return firstType;
        }

        public virtual DataType ProcessOperatorExpression(OperatorExpressionAst operatorExpression)
        {
            // 注意 https://c0.karenia.cc/c0/expr.html 参数类型必须是数值。
            // 所以如果有比较符号，那么它们都得是数值

            var firstType = ProcessWeakTerm(operatorExpression.WeakTerm);

            if (operatorExpression.OpTerms.Count == 0)
                return firstType;

            if (operatorExpression.OpTerms.Count > 1)
                throw new SemanticException($"List count >1");

            if (!DataTypeHelper.IsLongOrDouble(firstType))
                throw new SemanticException($"List is not empty, first type '{firstType}' should be int or double");

            var (op, weak) = operatorExpression.OpTerms[0];
            Debug.Assert(op.IsCompare);

            var weakTermType = ProcessWeakTerm(weak);
            if (weakTermType != firstType)
                throw new SemanticException(
                    $"A weak term '{weakTermType}' in list not matching first type '{firstType}'");

            // Check ok. Now comparing.
            var typec = DataTypeHelper.Suffix(firstType);

            if (CodeGenerationEnabled)
            {
                // first do cmp, approx. (a-b)
                Bucket.Add("cmp." + typec);

                switch (op.Value)
                {
                    case Operator.GreaterThan:
                        Bucket.Add("set.gt");
                        break;
                    case Operator.LessThan:
                        Bucket.Add("set.lt");
                        break;
                    case Operator.GreaterEqual:
                        // >= equivalent to (not <)
                        Bucket.Add("set.lt");
                        Bucket.Add("not");
                        break;
                    case Operator.LessEqual:
                        // <= equivalent to (not >)
                        Bucket.Add("set.gt");
                        Bucket.Add("not");
                        break;
                    case Operator.Equal:
                        Bucket.Add("not");
                        break;
                    case Operator.NotEqual:
                        // if not equal, a-b is not 0. (not 0 is true)
                        break;
                    default:
                        throw new Exception("Not Reached");
                };
            }

            return DataType.Bool;
        }
    }
}
