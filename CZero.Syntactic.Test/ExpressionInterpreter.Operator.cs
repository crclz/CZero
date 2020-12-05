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

        public static object Calculate(this FactorAst factor)
        {
            object value = factor.StrongFactor.Calculate();

            foreach (var item in factor.AsTypeList)
            {
                switch (item.TypeToken.Value)
                {
                    case "int":
                        value = (long)value;
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
            var resultObject = term.Calculate();

            if (resultObject is long longResult)
            {
                foreach (var (op, factor) in term.OpFactors)
                {
                    var factorVal = factor.Calculate();
                    if (!(factorVal is long longFactorVal))
                    {
                        throw new ArgumentException();
                    }

                    switch (op.Value)
                    {
                        case Lexical.Tokens.Operator.Mult:
                            longResult *= longFactorVal;
                            break;
                        case Lexical.Tokens.Operator.Divide:
                            longResult /= longFactorVal;
                            break;
                        default:
                            throw new ArgumentException();
                    }
                }

                return longResult;
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
            var resultObject = weak.Calculate();

            if (resultObject is long longResult)
            {
                foreach (var (op, term) in weak.OpTerms)
                {
                    var factorVal = term.Calculate();
                    if (!(factorVal is long longTermVal))
                    {
                        throw new ArgumentException();
                    }

                    switch (op.Value)
                    {
                        case Lexical.Tokens.Operator.Plus:
                            longResult += longTermVal;
                            break;
                        case Lexical.Tokens.Operator.Minus:
                            longResult -= longTermVal;
                            break;
                        default:
                            throw new ArgumentException();
                    }
                }

                return longResult;
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
