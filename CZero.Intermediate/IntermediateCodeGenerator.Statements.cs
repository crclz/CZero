using Ardalis.GuardClauses;
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
        /*
        stmt ->
              expr_stmt
            | decl_stmt
            | if_stmt
            | while_stmt
            | return_stmt
            | block_stmt
            | empty_stmt
            
            | break_stmt | continue_stmt
         */
        public virtual void ProcessStatement(StatementAst statement)
        {
            Guard.Against.Null(statement, nameof(statement));

            if (statement is BreakStatementAst breakStatement)
                ProcessBreakStatement(breakStatement);
            else if (statement is ContinueStatementAst continueStatement)
                ProcessContinueStatement(continueStatement);
            else if (statement is ExpressionStatementAst expressionStatement)
                ProcessExpressionStatement(expressionStatement);
            else if (statement is DeclarationStatementAst declarationStatement)
                ProcessDeclarationStatement(declarationStatement);
            else if (statement is IfStatementAst ifStatement)
                ProcessIfStatement(ifStatement);
            else if (statement is WhileStatementAst whileStatement)
                ProcessWhileStatement(whileStatement);
            else if (statement is ReturnStatementAst returnStatement)
                ProcessReturnStatement(returnStatement);
            else if (statement is BlockStatementAst blockStatement)
                ProcessBlockStatement(blockStatement);
            else if (statement is EmptyStatementAst emptyStatement)
                ProcessEmptyStatement(emptyStatement);
            else
                throw new ArgumentException($"Unknown statement type: {statement.GetType()}",
                    nameof(statement));
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
            var name = constDeclaration.Name.Value;
            if (SymbolScope.FindSymbolShallow(name, out Symbol existingSymbol))
            {
                throw new SemanticException(
                    $"Cannot const declare because of duplicated name. " +
                    $"Existing symbol type: {existingSymbol.GetType()}");
            }

            if (!new[] { "int", "double" }.Contains(constDeclaration.Type.Value))
                throw new SemanticException($"Type {constDeclaration.Type.Value} should be int or double");

            var declaringType = constDeclaration.Type.Value switch
            {
                "int" => DataType.Long,
                "double" => DataType.Double,
                _ => throw new Exception("Not Reached")
            };


            var initialExpressionType = ProcessExpression(constDeclaration.ValueExpression);

            if (declaringType != initialExpressionType)
            {
                throw new SemanticException(
                    $"DeclaringType: {declaringType}, InitialExpressionType: {initialExpressionType}");
            }

            // All check ok
            var symbol = new VariableSymbol(name, SymbolScope.IsRoot, true, declaringType);
            SymbolScope.AddSymbol(symbol);
        }

        public void ProcessIfStatement(IfStatementAst ifStatement)
        {
            Guard.Against.Null(ifStatement, nameof(ifStatement));
            // if_stmt -> 'if' expr block_stmt ('else' (block_stmt | if_stmt))?

            var conditionType = ProcessExpression(ifStatement.ConditionExpression);
            if (conditionType != DataType.Bool)
                throw new SemanticException($"If.Condition should be of bool type");
        }

        public void ProcessWhileStatement(WhileStatementAst whileStatement)
        {
            Guard.Against.Null(whileStatement, nameof(whileStatement));

            var conditionType = ProcessExpression(whileStatement.ConditionExpression);
            if (conditionType != DataType.Bool)
                throw new SemanticException($"If.Condition should be of bool type");
        }

        public void ProcessReturnStatement(ReturnStatementAst returnStatement)
        {
            Guard.Against.Null(returnStatement, nameof(returnStatement));
            // return_stmt -> 'return' expr? ';'
            // 无返回值 - 有返回值

            if (!IsInFunction)
                throw new SemanticException($"Cannot return out side of function defination");

            var expressionType = ProcessExpression(returnStatement.ReturnExpression);
            if (expressionType != CurrentFunction.ReturnType)
            {
                throw new SemanticException(
                    $"Should return {CurrentFunction.ReturnType}, but return {expressionType}");
            }

        }

        public void ProcessBlockStatement(BlockStatementAst blockStatement)
        {
            // block_stmt -> '{' stmt* '}'
            Guard.Against.Null(blockStatement, nameof(blockStatement));

            foreach (var statement in blockStatement.Statements)
            {
                ProcessStatement(statement);
            }
        }

        public void ProcessEmptyStatement(EmptyStatementAst emptyStatement)
        {
            // empty_stmt -> ';'
            Guard.Against.Null(emptyStatement, nameof(emptyStatement));
        }

        public void ProcessBreakStatement(BreakStatementAst breakStatement)
        {
            Guard.Against.Null(breakStatement, nameof(breakStatement));

            if (!IsInWhile)
                throw new SemanticException($"Not in while, cannot break");
        }

        public void ProcessContinueStatement(ContinueStatementAst continueStatement)
        {
            Guard.Against.Null(continueStatement, nameof(continueStatement));

            if (!IsInWhile)
                throw new SemanticException($"Not in while, cannot continue");
        }
    }
}
