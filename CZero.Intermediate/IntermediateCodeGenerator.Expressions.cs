﻿using Ardalis.GuardClauses;
using CZero.Intermediate.Instructions;
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
            Guard.Against.Null(expression, nameof(expression));

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

            var valueExpressionCode = ExpressionBucket.Pop();

            // All check ok

            if (CodeGenerationEnabled)
            {
                // load-variable-addr
                if (variableSymbol.IsGlobal)
                {
                    var id = variableSymbol.GlobalVariableBuilder.Id;
                    ExpressionBucket.Add(new object[] { "globa", id });
                }
                else
                {
                    if (variableSymbol.LocalLocation.IsArgument)
                    {
                        // 函数参数 (+1)
                        var id = variableSymbol.LocalLocation.Id;
                        if (CurrentFunction.ReturnType != DataType.Void)
                            id++;

                        var instruction = Instruction.Pack("arga", id);
                        instruction.Comment = variableSymbol.Name + " call-arg";
                        ExpressionBucket.Add(instruction);
                    }
                    else
                    {
                        // 局部变量
                        var id = variableSymbol.LocalLocation.Id;
                        ExpressionBucket.Add(new object[] { "loca", id });
                    }
                }

                // value-expr
                ExpressionBucket.AddRange(valueExpressionCode);

                // store.64
                ExpressionBucket.Add(new Instruction("store.64"));
            }

            return DataType.Void;
        }

        public IReadOnlyDictionary<string, string> StdLibReference = new Dictionary<string, string>
        {
            {"getint","scan.i" },{"getdouble", "scan.f"},{"getchar", "scan.c"},
            { "putint", "print.i"},{ "putdouble", "print.f"},{ "putchar","print.c" },
            { "putstr","print.s"},{ "putln","println"}
        };

        public bool IsStdLibCall(string s) => StdLibReference.ContainsKey(s);

        public virtual DataType ProcessCallExpression(CallExpressionAst callExpression)
        {
            var name = callExpression.Identifier.Value;

            if (!SymbolScope.FindSymbolDeep(name, out Symbol symbol))
                throw new SemanticException($"Symbol name '{name}' not found");

            if (!(symbol is FunctionSymbol functionSymbol))
                throw new SemanticException($"Symbol '{name}' is not function, cannot call.");

            // ret-space: 如果是stdlibs，不需要预留空间。如果是返回void的，也不用。
            if (CodeGenerationEnabled && !IsStdLibCall(functionSymbol.Name) && functionSymbol.ReturnType != DataType.Void)
                ExpressionBucket.Add(new object[] { "push", (long)0 });

            var needCount = functionSymbol.ParamTypes.Count;
            var providedCount = callExpression.HasParams ? callExpression.ParamList.Parameters.Count : 0;

            if (needCount != providedCount)
            {
                throw new SemanticException($"Function '{name}' needs {needCount} param(s)," +
                    $"but provided {providedCount} param(s)");
            }

            // Now, 0:0 or n:n. Check the types when n:n
            if (needCount != 0)
            {
                var providedTypes = new List<DataType>();

                foreach (var parameter in callExpression.ParamList.Parameters)
                {
                    var expressionType = ProcessExpression(parameter);
                    providedTypes.Add(expressionType);
                }

                Debug.Assert(functionSymbol.ParamTypes.Count == providedTypes.Count);

                if (!functionSymbol.IsParamTypeMatch(providedTypes))
                    throw new SemanticException($"Call to function '{name}' cannot match param types");
            }


            // call func id
            if (CodeGenerationEnabled)
            {
                if (!IsStdLibCall(functionSymbol.Name))
                {
                    var funcId = functionSymbol.Builder.Id;
                    ExpressionBucket.Add(Instruction.Pack("call", funcId));
                }
                else
                {
                    // stdlib calls

                    var opcode = StdLibReference[functionSymbol.Name];
                    Debug.Assert(opcode != null);

                    ExpressionBucket.Add(new Instruction(opcode));
                }
            }

            return functionSymbol.ReturnType;
        }

        public virtual DataType ProcessLiteralExpression(LiteralExpressionAst literalExpression)
        {
            if (literalExpression.Literal is UInt64LiteralToken intLiteral)
            {
                if (CodeGenerationEnabled)
                {
                    ExpressionBucket.Add(new object[] { "push", (long)intLiteral.Value });
                }
                return DataType.Long;
            }
            if (literalExpression.Literal is DoubleLiteralToken doubleLiteral)
            {
                if (CodeGenerationEnabled)
                {
                    ExpressionBucket.Add(new object[] { "push", doubleLiteral.Value });

                }
                return DataType.Double;
            }
            if (literalExpression.Literal is StringLiteralToken stringLiteral)
            {
                // 字符串应该存全局变量，并把地址（划掉）应该把全局变量号放在栈上
                if (CodeGenerationEnabled)
                {
                    var id = GlobalBuilder.RegisterStringConstant(stringLiteral.Value);

                    ExpressionBucket.Add(Instruction.Pack("push", (long)id));
                }

                return DataType.String;
            }
            if (literalExpression.Literal is CharLiteralToken charLiteral)
            {
                if (CodeGenerationEnabled)
                {
                    ExpressionBucket.Add(new object[] { "push", (long)charLiteral.Value });
                }

                return DataType.Long;
            }
            throw new ArgumentException($"Unknown literal token type: {literalExpression.GetType()}");

        }

        public virtual DataType ProcessIdentExpression(IdentExpressionAst identExpression)
        {
            var name = identExpression.IdentifierToken.Value;
            if (!SymbolScope.FindSymbolDeep(name, out Symbol symbol))
                throw new SemanticException($"Symbol {name} not exist");

            if (!(symbol is VariableSymbol variableSymbol))
                throw new SemanticException($"Symbol {name} is not const/var");

            // All check ok

            if (CodeGenerationEnabled)
            {
                // load-variable-addr
                if (variableSymbol.IsGlobal)
                {
                    var id = variableSymbol.GlobalVariableBuilder.Id;
                    ExpressionBucket.Add(new object[] { "globa", id });
                }
                else
                {
                    if (variableSymbol.LocalLocation.IsArgument)
                    {
                        var id = variableSymbol.LocalLocation.Id;
                        if (CurrentFunction.ReturnType != DataType.Void)
                            id++;// because arg0 is ret val
                        var instruction = Instruction.Pack("arga", id);
                        instruction.Comment = variableSymbol.Name + " (ident-expr)";
                        ExpressionBucket.Add(instruction);
                    }
                    else
                    {
                        // 局部变量
                        var id = variableSymbol.LocalLocation.Id;
                        ExpressionBucket.Add(new object[] { "loca", id });
                    }
                }

                // de-reference
                ExpressionBucket.Add(Instruction.Pack("load.64"));
            }

            return variableSymbol.Type;
        }

        public virtual DataType ProcessGroupExpression(GroupExpressionAst groupExpression)
        {
            return ProcessExpression(groupExpression.Expression);
        }
    }
}
