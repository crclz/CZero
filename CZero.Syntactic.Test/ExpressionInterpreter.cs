﻿using CZero.Lexical.Tokens;
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
        public static object Calculate(this ExpressionAst expression)
        {
            switch (expression)
            {
                case OperatorExpressionAst operatorExpression:
                    return operatorExpression.Calculate();
                case AssignExpressionAst assignExpression:
                    return assignExpression.Calculate();
                case CallExpressionAst callExpression:
                    return callExpression.Calculate();
                case LiteralExpressionAst literalExpression:
                    return literalExpression.Calculate();
                case IdentExpressionAst identExpression:
                    return identExpression.Calculate();
                case GroupExpressionAst groupExpression:
                    return groupExpression.Calculate();
                default:
                    throw new ArgumentException();
            }
        }

        public static object Calculate(this AssignExpressionAst assignExpression)
        {
            return null;
        }

        public static object Calculate(this CallExpressionAst callExpression)
        {
            if (callExpression.Identifier.Value == "intsum")
            {
                int s = 0;
                if (callExpression.ParamList != null)
                {
                    foreach (var arg in callExpression.ParamList.Parameters)
                    {
                        int val = (int)arg.Calculate();
                        s += val;
                    }
                }

                return s;
            }

            throw new NotImplementedException();
        }

        public static object Calculate(this LiteralExpressionAst literalExpression)
        {
            switch (literalExpression.Literal)
            {
                case StringLiteralToken sl:
                    throw new ArgumentException();
                case UInt64LiteralToken ul:
                    return (int)ul.Value;
                case DoubleLiteralToken dl:
                    return dl.Value;
                default:
                    throw new ArgumentException();
            }
        }

        public static object Calculate(this IdentExpressionAst identExpression)
        {
            if (identExpression.IdentifierToken.Value == "i1")
                return 1;
            if (identExpression.IdentifierToken.Value == "i2")
                return 2;
            if (identExpression.IdentifierToken.Value == "i5")
                return 5;

            if (identExpression.IdentifierToken.Value == "d1")
                return 1.0;
            if (identExpression.IdentifierToken.Value == "d2")
                return 2.0;
            if (identExpression.IdentifierToken.Value == "d5")
                return 5.0;

            throw new NotImplementedException();
        }

        public static object Calculate(this GroupExpressionAst groupExpression)
        {
            return groupExpression.Expression.Calculate();
        }
    }
}
