using CZero.Intermediate.Symbols;
using CZero.Lexical.Tokens;
using CZero.Syntactic.Ast.Expressions;
using CZero.Syntactic.Ast.Expressions.OperatorExpression;
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
        public virtual DataType ProcessStatement(StatementAst statement)
        {
            throw new NotImplementedException();
        }

        public virtual void ProcessExpressionStatement(ExpressionStatementAst expressionStatement)
        {
            // 表达式语句由 表达式 后接分号组成。表达式如果有值，值将会被丢弃。

            var expressionReturnType = ProcessExpression(expressionStatement.Expression);

            // TODO: 代码生成，如果不是返回void，则丢弃栈顶的值
        }

        public virtual void ProcessDeclarationStatement(DeclarationStatementAst declarationStatement)
        {
            if (declarationStatement is LetDeclarationStatementAst letDeclaration)
                ProcessLetDeclarationStatement(letDeclaration);
            else if (declarationStatement is ConstDeclarationStatementAst constDeclaration)
                ProcessConstDeclarationStatement(constDeclaration);
            else
                throw new ArgumentException(nameof(declarationStatement));
        }

        public virtual void ProcessLetDeclarationStatement(LetDeclarationStatementAst letDeclaration)
        {
            var name = letDeclaration.Name.Value;
            if (SymbolScope.FindSymbolShallow(name, out Symbol existingSymbol))
            {
                throw new SemanticException(
                    $"Cannot let declare because of duplicated name. " +
                    $"Existing symbol type: {existingSymbol.GetType()}");
            }

            if (!new[] { "int", "double" }.Contains(letDeclaration.Type.Value))
                throw new SemanticException($"Type {letDeclaration.Type.Value} should be int or double");

            var declaringType = letDeclaration.Type.Value switch
            {
                "int" => DataType.Long,
                "double" => DataType.Double,
                _ => throw new Exception("Not Reached")
            };

            if (letDeclaration.HasInitialExpression)
            {
                var initialExpressionType = ProcessExpression(letDeclaration.InitialExpression);

                if (declaringType != initialExpressionType)
                {
                    throw new SemanticException(
                        $"DeclaringType: {declaringType}, InitialExpressionType: {initialExpressionType}");
                }
            }

            // Add to symbol table
            var symbol = new VariableSymbol(name, isGlobal: SymbolScope.IsRoot, isConstant: false, declaringType);
            SymbolScope.AddSymbol(symbol);
        }

        public virtual void ProcessConstDeclarationStatement(ConstDeclarationStatementAst constDeclaration)
        {
            throw new NotImplementedException();
        }
    }
}
