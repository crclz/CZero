﻿using Ardalis.GuardClauses;
using CZero.Intermediate.Instructions;
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
        public virtual bool ProcessStatement(StatementAst statement)
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
            {
                return ProcessIfStatement(ifStatement);
            }
            else if (statement is WhileStatementAst whileStatement)
                ProcessWhileStatement(whileStatement);
            else if (statement is ReturnStatementAst returnStatement)
            {
                ProcessReturnStatement(returnStatement);
                return true;
            }
            else if (statement is BlockStatementAst blockStatement)
            {
                return ProcessBlockStatement(blockStatement);
            }
            else if (statement is EmptyStatementAst emptyStatement)
                ProcessEmptyStatement(emptyStatement);
            else
                throw new ArgumentException($"Unknown statement type: {statement.GetType()}",
                    nameof(statement));

            return false;
        }

        public virtual void ProcessExpressionStatement(ExpressionStatementAst expressionStatement)
        {
            // 表达式语句由 表达式 后接分号组成。表达式如果有值，值将会被丢弃。

            var expressionReturnType = ProcessExpression(expressionStatement.Expression);

            // 因为表达式还在临时bucket里面，所以需要pop出来放在函数bucket里面
            if (CodeGenerationEnabled)
                CurrentFunction.Builder.Bucket.AddRange(ExpressionBucket.Pop());

            // 代码生成，如果不是返回void，则丢弃栈顶的值
            if (CodeGenerationEnabled)
            {
                if (expressionReturnType != DataType.Void)
                {
                    CurrentFunction.Builder.Bucket.AddSingle("pop");
                }
            }
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
            var symbol = new VariableSymbol(name, isGlobal: !IsInFunction, isConstant: false, declaringType);
            SymbolScope.AddSymbol(symbol);

            if (CodeGenerationEnabled)
            {
                if (!letDeclaration.HasInitialExpression)
                    Debug.Assert(ExpressionBucket.InstructionList.Count == 0);

                if (symbol.IsGlobal)
                {
                    GlobalBuilder.RegisterGlobalVariable(symbol);

                    if (letDeclaration.HasInitialExpression)
                    {
                        symbol.GlobalVariableBuilder.LoadValueInstructions = ExpressionBucket.Pop();
                        Debug.Assert(symbol.GlobalVariableBuilder.LoadValueInstructions.Count > 0);
                    }
                }
                else
                {
                    CurrentFunction.Builder.RegisterLocalVariable(symbol);

                    // load address
                    CurrentFunction.Builder.Bucket.Add(new object[] { "loca", symbol.LocalLocation.Id });

                    if (letDeclaration.HasInitialExpression)
                    {
                        // load init expr
                        CurrentFunction.Builder.Bucket.AddRange(ExpressionBucket.Pop());
                    }
                    else
                    {
                        // load zero value
                        CurrentFunction.Builder.Bucket.Add(new object[] { "push", (long)0 });
                    }

                    // set memory value
                    CurrentFunction.Builder.Bucket.AddSingle("store.64");
                }
            }
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
            var symbol = new VariableSymbol(name, !IsInFunction, true, declaringType);
            SymbolScope.AddSymbol(symbol);

            if (CodeGenerationEnabled)
            {
                if (symbol.IsGlobal)
                {
                    GlobalBuilder.RegisterGlobalVariable(symbol);

                    symbol.GlobalVariableBuilder.LoadValueInstructions = ExpressionBucket.Pop();
                    Debug.Assert(symbol.GlobalVariableBuilder.LoadValueInstructions.Count > 0);
                }
                else
                {
                    CurrentFunction.Builder.RegisterLocalVariable(symbol);

                    // load address
                    CurrentFunction.Builder.Bucket.Add(new object[] { "loca", symbol.LocalLocation.Id });

                    // load init expr
                    CurrentFunction.Builder.Bucket.AddRange(ExpressionBucket.Pop());

                    // set memory value
                    CurrentFunction.Builder.Bucket.AddSingle("store.64");
                }
            }
        }

        public bool ProcessIfStatement(IfStatementAst ifStatement)
        {
            // 能脱离函数的充要条件：
            // 1. 不能只有if, 要有else。只有if 相当于 可能脱离。“可能”视为0.
            // 2. if-block可以脱离 && else也可以脱离

            Guard.Against.Null(ifStatement, nameof(ifStatement));
            // if_stmt -> 'if' expr block_stmt ('else' (block_stmt | if_stmt))?

            var conditionType = ProcessExpression(ifStatement.ConditionExpression);
            if (!(conditionType == DataType.Bool ||
                conditionType == DataType.Double ||
                conditionType == DataType.Long))
            {
                throw new SemanticException(
                    $"If.Condition should be of bool,double or long type. Provided: {conditionType}");
            }


            // codegen: cond-expr
            if (CodeGenerationEnabled)
            {
                CurrentFunction.Builder.Bucket.AddRange(ExpressionBucket.Pop());
            }

            bool canReturn1 = false;
            bool canReturn2 = false;

            if (ifStatement.HasElseAndFollowing)
            {
                var else0 = new Instruction("nop");
                var done0 = new Instruction("nop");

                // jmp-if-false .ELSE0
                if (CodeGenerationEnabled)
                    CurrentFunction.Builder.Bucket.Add(new object[] { "br.false", else0 });

                // if-block
                canReturn1 = ProcessBlockStatement(ifStatement.BlockStatement);

                // jmp .DONE0
                if (CodeGenerationEnabled)
                    CurrentFunction.Builder.Bucket.Add(new object[] { "br", done0 });

                // .ELSE0: nop
                if (CodeGenerationEnabled)
                    CurrentFunction.Builder.Bucket.Add(else0);

                // else-block
                if (ifStatement.FollowingIf != null)
                {
                    // if-else-if
                    canReturn2 = ProcessIfStatement(ifStatement.FollowingIf);
                }
                else
                {
                    // if-else
                    canReturn2 = ProcessBlockStatement(ifStatement.FollowingBlock);
                }

                // .DONE0: nop
                if (CodeGenerationEnabled)
                    CurrentFunction.Builder.Bucket.Add(done0);

                return canReturn1 && canReturn2;
            }
            else
            {
                var doneInstruction = new Instruction("nop");
                if (CodeGenerationEnabled)
                    CurrentFunction.Builder.Bucket.Add(new object[] { "br.false", doneInstruction });

                // if-block
                canReturn1 = ProcessBlockStatement(ifStatement.BlockStatement);

                // .DONE
                if (CodeGenerationEnabled)
                    CurrentFunction.Builder.Bucket.Add(doneInstruction);
            }

            return false;
        }

        public void ProcessWhileStatement(WhileStatementAst whileStatement)
        {
            Guard.Against.Null(whileStatement, nameof(whileStatement));

            var conditionType = ProcessExpression(whileStatement.ConditionExpression);

            if (!(conditionType == DataType.Bool ||
                conditionType == DataType.Double ||
                conditionType == DataType.Long))
            {
                throw new SemanticException(
                    $"While condition should be of bool, double or long type. Provided: {conditionType}");
            }

            var doneLabel = new Instruction("nop");

            var startLabel = new Instruction("nop");
            if (CodeGenerationEnabled)
                CurrentFunction.Builder.Bucket.Add(startLabel);

            // cond-expr
            if (CodeGenerationEnabled)
                CurrentFunction.Builder.Bucket.AddRange(ExpressionBucket.Pop());

            // jump(if false) to done
            if (CodeGenerationEnabled)
                CurrentFunction.Builder.Bucket.Add(new object[] { "br.false", doneLabel });

            // # while-block
            EnterWhileDefination(new Builders.WhileBuilder(CurrentWhile, startLabel, doneLabel));

            ProcessBlockStatement(whileStatement.WhileBlock);

            LeaveWhileDefination();

            // jmp .START
            if (CodeGenerationEnabled)
                CurrentFunction.Builder.Bucket.Add(Instruction.Pack("br", startLabel));

            // .DONE:nop
            if (CodeGenerationEnabled)
                CurrentFunction.Builder.Bucket.Add(doneLabel);
        }

        public void ProcessReturnStatement(ReturnStatementAst returnStatement)
        {
            Guard.Against.Null(returnStatement, nameof(returnStatement));
            // return_stmt -> 'return' expr? ';'
            // 无返回值 - 有返回值

            if (!IsInFunction)
                throw new SemanticException($"Cannot return out side of function defination");

            var sessId = Guid.NewGuid().ToString()
;
            // load-retval-addr
            if (CodeGenerationEnabled && CurrentFunction.ReturnType != DataType.Void)
            {
                var instruction = Instruction.Pack("arga", 0);
                instruction.Comment = "load retval addr " + sessId;
                CurrentFunction.Builder.Bucket.Add(instruction);
            }

            DataType actualReturnType;
            if (returnStatement.ReturnExpression != null)
                actualReturnType = ProcessExpression(returnStatement.ReturnExpression);
            else
                actualReturnType = DataType.Void;

            if (actualReturnType != CurrentFunction.ReturnType)
            {
                throw new SemanticException(
                    $"Should return {CurrentFunction.ReturnType}, but return {actualReturnType}");
            }

            // returning-expr;
            if (actualReturnType == DataType.Void)
                Debug.Assert(ExpressionBucket.InstructionList.Count == 0);

            if (CodeGenerationEnabled)
            {
                var exprCode = ExpressionBucket.Pop();
                exprCode.ForEach(p => p.Comment += " " + sessId);
                CurrentFunction.Builder.Bucket.AddRange(exprCode);
            }

            // write-retval
            if (CodeGenerationEnabled && CurrentFunction.ReturnType != DataType.Void)
            {
                if (CodeGenerationEnabled)
                    CurrentFunction.Builder.Bucket.Add(new Instruction("store.64"));
            }

            // ret
            if (CodeGenerationEnabled)
            {
                CurrentFunction.Builder.Bucket.Add(new Instruction("ret")
                {
                    Comment = " " + sessId
                });
            }
        }

        public virtual bool ProcessBlockStatement(BlockStatementAst blockStatement,
            bool suppressNewScopeCreation = false)
        {
            // block_stmt -> '{' stmt* '}'
            Guard.Against.Null(blockStatement, nameof(blockStatement));

            if (!suppressNewScopeCreation)
            {
                SymbolScope = SymbolScope.CreateChildScope();
            }

            bool blockCanReturn = false;

            foreach (var statement in blockStatement.Statements)
            {
                bool canReturn = ProcessStatement(statement);
                blockCanReturn |= canReturn;
            }

            if (!suppressNewScopeCreation)
            {
                SymbolScope = SymbolScope.ParentScope;
            }

            return blockCanReturn;
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

            // jmp CurrentWhile.Done
            Debug.Assert(CurrentWhile.DoneLabel != null);

            if (CodeGenerationEnabled)
                CurrentFunction.Builder.Bucket.Add(Instruction.Pack("br", CurrentWhile.DoneLabel));
        }

        public void ProcessContinueStatement(ContinueStatementAst continueStatement)
        {
            Guard.Against.Null(continueStatement, nameof(continueStatement));

            if (!IsInWhile)
                throw new SemanticException($"Not in while, cannot continue");

            // jmp CurrentWhile.Start
            Debug.Assert(CurrentWhile.StartLabel != null);

            if (CodeGenerationEnabled)
                CurrentFunction.Builder.Bucket.Add(Instruction.Pack("br", CurrentWhile.StartLabel));
        }
    }
}
