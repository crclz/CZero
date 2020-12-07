using CZero.Lexical.Tokens;
using CZero.Syntactic.Ast.Expressions;
using CZero.Syntactic.Ast.Expressions.OperatorExpression;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace CZero.Intermediate
{
    partial class IntermediateCodeGenerator
    {
        public virtual DataType ProcessExpression(ExpressionAst expression)
        {
            if (expression is OperatorExpressionAst operatorExpression)
                return ProcessOperatorExpression(operatorExpression);
            if (expression is AssignExpressionAst assignExpression)
                return ProcessAssignExpression(assignExpression);
            if (expression is CallExpressionAst callExpression)
                return ProcessCallExpression(callExpression);
            if (expression is LiteralExpressionAst literalExpression)
                return ProcessLiteralExpression(literalExpression);
            if (expression is IdentExpressionAst identExpression)
                return ProcessIdentExpression(identExpression);
            if (expression is GroupExpressionAst groupExpression)
                return ProcessGroupExpression(groupExpression);

            throw new ArgumentException("Unknown expression type" + expression.GetType(), nameof(expression));
        }

        public virtual DataType ProcessAssignExpression(AssignExpressionAst assignExpression)
        {
            // 沾点符号表了，所以下个阶段实现
            return DataType.Void;
        }

        public virtual DataType ProcessCallExpression(CallExpressionAst callExpression)
        {
            // 沾点符号表了，所以下个阶段实现
            throw new NotImplementedException();
        }

        public virtual DataType ProcessLiteralExpression(LiteralExpressionAst literalExpression)
        {
            if (literalExpression.Literal is UInt64LiteralToken)
                return DataType.Long;
            if (literalExpression.Literal is DoubleLiteralToken)
                return DataType.Double;
            if (literalExpression.Literal is StringLiteralToken)
                return DataType.String;
            if (literalExpression.Literal is CharLiteralToken)
                return DataType.Char;
            throw new ArgumentException($"Unknown literal token type: {literalExpression.GetType()}");
        }

        public virtual DataType ProcessIdentExpression(IdentExpressionAst identExpression)
        {
            // 沾点符号表了，所以下个阶段实现
            throw new NotImplementedException();
        }

        public virtual DataType ProcessGroupExpression(GroupExpressionAst groupExpression)
        {
            // 沾点符号表了，所以下个阶段实现
            throw new NotImplementedException();
        }
    }
}
