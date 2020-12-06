using CZero.Lexical.Tokens;
using CZero.Syntactic.Ast.Functions;
using CZero.Syntactic.Ast.Statements;
using System;
using System.Collections.Generic;
using System.Text;

namespace CZero.Syntactic
{
    public partial class SyntacticAnalyzer
    {
        public bool TryFunctionParam(out FunctionParamAst functionParam)
        {
            // 'const'? IDENT ':' ty
            var oldCursor = _reader._cursor;

            if (!_reader.AdvanceIfCurrentIsKeyword(out KeywordToken @const, Keyword.Const))
                goto fail;
            if (!_reader.AdvanceIfCurrentIsType(out IdentifierToken name))
                goto fail;
            if (!_reader.AdvanceIfCurrentIsOperator(out OperatorToken colon, Operator.Colon))
                goto fail;
            if (!_reader.AdvanceIfCurrentIsType(out IdentifierToken type))
                goto fail;

            functionParam = new FunctionParamAst(@const, name, colon, type);
            return true;

        fail:
            functionParam = null;
            return restoreCursor(oldCursor);
        }

        public bool TryFunctionParamList(out FunctionParamListAst functionParamList)
        {
            // function_param (',' function_param)*
            var oldCursor = _reader._cursor;

            var paramList = new List<FunctionParamAst>();

            // first param is required
            if (!TryFunctionParam(out FunctionParamAst firstParam))
                goto fail;
            paramList.Add(firstParam);

            // other params are optional
            while (TryFunctionParam(out FunctionParamAst aParam))
                paramList.Add(aParam);

            functionParamList = new FunctionParamListAst(paramList);
            return true;

        fail:
            functionParamList = null;
            return restoreCursor(oldCursor);
        }

        public bool TryFunction(out FunctionAst function)
        {
            // 'fn' IDENT '(' function_param_list? ')' '->' ty block_stmt
            var oldCursor = _reader._cursor;

            if (!_reader.AdvanceIfCurrentIsKeyword(out KeywordToken fn, Keyword.Fn))
                goto fail;
            if (!_reader.AdvanceIfCurrentIsType(out IdentifierToken name))
                goto fail;
            if (!_reader.AdvanceIfCurrentIsOperator(out OperatorToken leftParen, Operator.LeftParen))
                goto fail;

            TryFunctionParamList(out FunctionParamListAst functionParamList);

            if (!_reader.AdvanceIfCurrentIsOperator(out OperatorToken rightParen, Operator.RightParen))
                goto fail;
            if (!_reader.AdvanceIfCurrentIsOperator(out OperatorToken arrow, Operator.Arrow))
                goto fail;
            if (!_reader.AdvanceIfCurrentIsType(out IdentifierToken returnType))
                goto fail;
            if (!TryBlockStatement(out BlockStatementAst bodyBlock))
                goto fail;

            function = new FunctionAst(
                fn, name, leftParen, functionParamList,
                rightParen, arrow, returnType, bodyBlock);
            return true;

        fail:
            function = null;
            return restoreCursor(oldCursor);
        }
    }
}
