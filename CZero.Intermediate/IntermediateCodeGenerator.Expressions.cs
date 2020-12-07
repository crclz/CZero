using CZero.Intermediate.Symbols;
using CZero.Lexical.Tokens;
using CZero.Syntactic.Ast.Expressions;
using CZero.Syntactic.Ast.Expressions.OperatorExpression;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
            // TODO: 沾点代码生成了，所以下个阶段补充

            // IDENT '=' expr
            // IDENT 是可以赋值的变量，expr是int或者double

            var name = assignExpression.Identifier.Value;

            if (!SymbolScope.FindSymbolDeep(assignExpression.Identifier.Value, out Symbol symbol))
                throw new SemanticException($"Symbol name '{name}' not found");

            if (!(symbol is VariableSymbol variableSymbol))
                throw new SemanticException($"Symbol '{name}' is function symbol, cannot assign value");

            if (variableSymbol.IsConstant)
                throw new SemanticException($"Symbol '{name}' is constant, cannot assign value");

            var valueType = ProcessExpression(assignExpression.Expression);
            if (valueType != variableSymbol.Type)
                throw new SemanticException(
                    $"Variable '{name}' is type '{variableSymbol.Type}', but expression is '{valueType}'");

            // All check ok

            // TODO: 沾点代码生成了，所以下个阶段补充

            return DataType.Void;
        }

        public virtual DataType ProcessCallExpression(CallExpressionAst callExpression)
        {
            // TODO: 沾点代码生成了，所以下个阶段补充

            var name = callExpression.Identifier.Value;

            if (!SymbolScope.FindSymbolDeep(name, out Symbol symbol))
                throw new SemanticException($"Symbol name '{name}' not found");

            if (!(symbol is FunctionSymbol functionSymbol))
                throw new SemanticException($"Symbol '{name}' is not function, cannot call.");

            if (functionSymbol.ParamTypes.Count == 0 && callExpression.HasParams)
                throw new SemanticException($"Function '{name}' has no params, " +
                    $"but provided {callExpression.ParamList.Parameters.Count} parameters");

            if (functionSymbol.ParamTypes.Count > 0 && !callExpression.HasParams)
                throw new SemanticException($"Function '{name}' needs param(s)," +
                    $"but expression did not provide any parameters");

            // 注意，现在，0:0 或者 n:m
            if (functionSymbol.ParamTypes.Count == 0)// 0:0
                Debug.Assert(!callExpression.HasParams);
            else
            {
                Debug.Assert(callExpression.HasParams);
                if (functionSymbol.ParamTypes.Count != callExpression.ParamList.Parameters.Count)
                    throw new SemanticException(
                        $"Function '{name}' needs {functionSymbol.ParamTypes.Count} params," +
                        $"But provided {callExpression.ParamList.Parameters.Count} params");
            }

            // Now, 0:0 or n:n. Check the types when n:n
            var providedTypes = new List<DataType>();
            if (callExpression.HasParams)
            {
                foreach (var parameter in callExpression.ParamList.Parameters)
                {
                    var expressionType = ProcessExpression(parameter);
                    providedTypes.Add(expressionType);
                }

                Debug.Assert(functionSymbol.ParamTypes.Count == providedTypes.Count);

                if (!functionSymbol.IsParamTypeMatch(providedTypes))
                    throw new SemanticException($"Call to function '{name}' cannot match param types");
            }


            // Function types check ok
            // TODO: 沾点代码生成了，所以下个阶段补充

            return functionSymbol.ReturnType;
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
