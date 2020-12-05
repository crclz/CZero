using CZero.Syntactic.Ast.Expressions;
using CZero.Syntactic.Ast.Expressions.OperatorExpression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CZero.Syntactic.Test
{
    static partial class ExpressionInterpreter
    {
        public static object Calculate(this StrongFactorAst strongFactor)
        {
            return Calculate(strongFactor.SingleExpression);
        }

        public static object Calculate(this GoodFactorAst goodFactor)
        {
            var val = goodFactor.StrongFactor.Calculate();

            int sign = 1;
            if (goodFactor.IsNegative)
                sign = -1;

            if (val is int intVal)
                return sign * intVal;
            if (val is double doubleValue)
                return sign * doubleValue;
            else
                throw new ArgumentException();
        }

        public static object Calculate(this FactorAst factor)
        {
            object value = factor.GoodFactor.Calculate();

            foreach (var item in factor.AsTypeList)
            {
                switch (item.TypeToken.Value)
                {
                    case "int":
                        value = (int)value;
                        break;
                    case "double":
                        value = (double)value;
                        break;
                    default:
                        throw new ArgumentException();
                }
            }

            return value;
        }

        public static object Calculate(this TermAst term)
        {
            var resultObject = term.Factor.Calculate();

            if (resultObject is int intResult)
            {
                foreach (var (op, factor) in term.OpFactors)
                {
                    var factorVal = factor.Calculate();
                    if (!(factorVal is int intFactorVal))
                    {
                        throw new ArgumentException();
                    }

                    switch (op.Value)
                    {
                        case Lexical.Tokens.Operator.Mult:
                            intResult *= intFactorVal;
                            break;
                        case Lexical.Tokens.Operator.Divide:
                            intResult /= intFactorVal;
                            break;
                        default:
                            throw new ArgumentException();
                    }
                }

                return intResult;
            }
            else if (resultObject is double doubleResult)
            {
                foreach (var (op, factor) in term.OpFactors)
                {
                    var factorVal = factor.Calculate();
                    if (!(factorVal is double doubleFactorVal))
                    {
                        throw new ArgumentException();
                    }

                    switch (op.Value)
                    {
                        case Lexical.Tokens.Operator.Mult:
                            doubleResult *= doubleFactorVal;
                            break;
                        case Lexical.Tokens.Operator.Divide:
                            doubleResult /= doubleFactorVal;
                            break;
                        default:
                            throw new ArgumentException();
                    }
                }

                return doubleResult;
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public static object Calculate(this WeakTermAst weak)
        {
            var resultObject = weak.Term.Calculate();

            if (resultObject is int intResult)
            {
                foreach (var (op, term) in weak.OpTerms)
                {
                    var factorVal = term.Calculate();
                    if (!(factorVal is int intTermVal))
                    {
                        throw new ArgumentException();
                    }

                    switch (op.Value)
                    {
                        case Lexical.Tokens.Operator.Plus:
                            intResult += intTermVal;
                            break;
                        case Lexical.Tokens.Operator.Minus:
                            intResult -= intTermVal;
                            break;
                        default:
                            throw new ArgumentException();
                    }
                }

                return intResult;
            }
            else if (resultObject is double doubleResult)
            {
                foreach (var (op, term) in weak.OpTerms)
                {
                    var factorVal = term.Calculate();
                    if (!(factorVal is double doubleFactorVal))
                    {
                        throw new ArgumentException();
                    }

                    switch (op.Value)
                    {
                        case Lexical.Tokens.Operator.Plus:
                            doubleResult += doubleFactorVal;
                            break;
                        case Lexical.Tokens.Operator.Minus:
                            doubleResult -= doubleFactorVal;
                            break;
                        default:
                            throw new ArgumentException();
                    }
                }

                return doubleResult;
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public static object Calculate(this OperatorExpressionAst operatorExpression)
        {
            // weak_term { 比较符 weak_term }
            var weakVal = operatorExpression.WeakTerm.Calculate();

            if (operatorExpression.OpTerms.Count > 0)
            {
                throw new ArgumentException("comparison unsupported");
            }

            return weakVal;
        }
    }
}
