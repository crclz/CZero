using CZero.Lexical.Tokens;
using CZero.Syntactic.Ast.Statements;
using System;
using System.Collections.Generic;
using System.Text;

namespace CZero.Syntactic.Ast.Functions
{
    public class FunctionAst : Ast
    {
        // 'fn' IDENT '(' function_param_list? ')' '->' ty block_stmt

        public KeywordToken Fn { get; }
        public IdentifierToken Name { get; }
        public OperatorToken LeftParen { get; }

        /// <summary>
        /// Optional
        /// </summary>
        public FunctionParamListAst FunctionParamList { get; }

        public OperatorToken RightParen { get; }
        public OperatorToken Arrow { get; }
        public IdentifierToken ReturnType { get; }
        public BlockStatementAst BodyBlock { get; }

        public bool HasParams => FunctionParamList != null;

        public FunctionAst(
            KeywordToken fn, IdentifierToken name, OperatorToken leftParen,
            FunctionParamListAst functionParamList, OperatorToken rightParen,
            OperatorToken arrow, IdentifierToken returnType, BlockStatementAst bodyBlock)
        {
            Fn = fn ?? throw new ArgumentNullException(nameof(fn));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            LeftParen = leftParen ?? throw new ArgumentNullException(nameof(leftParen));
            FunctionParamList = functionParamList;
            RightParen = rightParen ?? throw new ArgumentNullException(nameof(rightParen));
            Arrow = arrow ?? throw new ArgumentNullException(nameof(arrow));
            ReturnType = returnType ?? throw new ArgumentNullException(nameof(returnType));
            BodyBlock = bodyBlock ?? throw new ArgumentNullException(nameof(bodyBlock));
        }
    }
}
