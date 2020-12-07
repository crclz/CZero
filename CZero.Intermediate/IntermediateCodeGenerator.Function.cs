using Ardalis.GuardClauses;
using CZero.Intermediate.Symbols;
using CZero.Lexical.Tokens;
using CZero.Syntactic.Ast;
using CZero.Syntactic.Ast.Expressions;
using CZero.Syntactic.Ast.Expressions.OperatorExpression;
using CZero.Syntactic.Ast.Functions;
using CZero.Syntactic.Ast.Statements;
using CZero.Syntactic.Ast.Statements.Declarative;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CZero.Intermediate
{
    partial class IntermediateCodeGenerator
    {
        public void ProcessFunction(FunctionAst functionAst)
        {
            Guard.Against.Null(functionAst, nameof(functionAst));

            if (SymbolScope.FindSymbolDeep(functionAst.Name.Value, out Symbol symbol))
                throw new SemanticException($"Symbol already exist: {symbol.Name}, {symbol.GetType()}");

            var returningTypeName = functionAst.ReturnType.Value;
            if (!new[] { "int", "double" }.Contains(returningTypeName))
                throw new SemanticException($"Bad returning type: {returningTypeName}");

            if (functionAst.HasParams)
            {
                foreach (var parameter in functionAst.FunctionParamList.FunctionParams)
                {
                    var paramType = parameter.Type.Value switch
                    {
                        "int" => DataType.Long,
                        "double" => DataType.Double,
                        _ => throw new SemanticException($"Bad param type: {parameter.Type.Value}")
                    };

                    var name = parameter.Name.Value;
                    if (SymbolScope.FindSymbolShallow(name, out Symbol _))
                        throw new SemanticException($"Exist symbol: {name}");

                    // Add to scope
                    SymbolScope.AddSymbol(new VariableSymbol(name, false, parameter.IsConstant, paramType));
                }
            }

            // All check ok

            // Scope of function is created by the caller
            ProcessBlockStatement(functionAst.BodyBlock, suppressNewScopeCreation: true);
        }

        public void ProcessProgram(ProgramAst programAst)
        {
            foreach (var element in programAst.Elements)
            {
                if (element is DeclarationStatementAst declarationStatement)
                    ProcessDeclarationStatement(declarationStatement);
                else if (element is FunctionAst function)
                {
                    SymbolScope = SymbolScope.CreateChildScope();
                    ProcessFunction(function);
                    SymbolScope = SymbolScope.ParentScope;
                }
                else
                    throw new ArgumentException($"Unknown ast type of one element: {element.GetType()}");

            }
        }
    }
}
