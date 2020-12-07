using CZero.Lexical.Tokens;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace CZero.Syntactic.Ast.Expressions
{
    public class CallExpressionAst : ExpressionAst
    {
        public IdentifierToken Identifier { get; }

        public OperatorToken LeftParen { get; }

        // This is Optional
        public CallParamListAst ParamList { get; }

        public bool HasParams => ParamList != null;

        public OperatorToken RightParen { get; }

        public CallExpressionAst(
            IdentifierToken identifier, OperatorToken leftParen,
            CallParamListAst paramList, OperatorToken rightParen)
        {
            Identifier = identifier ?? throw new ArgumentNullException(nameof(identifier));
            LeftParen = leftParen ?? throw new ArgumentNullException(nameof(leftParen));
            ParamList = paramList;// optional
            RightParen = rightParen ?? throw new ArgumentNullException(nameof(rightParen));

            if (leftParen.Value != Operator.LeftParen)
                throw new ArgumentException();

            if (rightParen.Value != Operator.RightParen)
                throw new ArgumentException();
        }
    }
}
