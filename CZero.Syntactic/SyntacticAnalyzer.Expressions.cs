using CZero.Lexical.Tokens;
using CZero.Syntactic.Ast.Expressions;
using CZero.Syntactic.Ast.Expressions.OperatorExpression;
using System;
using System.Collections.Generic;
using System.Text;

namespace CZero.Syntactic
{
    public partial class SyntacticAnalyzer
    {
        private bool restoreCursor(int oldCursor)
        {
            _reader.SetCursor(oldCursor);
            return !true;
        }



        internal bool TryNegateExpression(out NegateExpressionAst e)
        {
            var oldCursor = _reader._cursor;

            // -> '-' expr

            if (!_reader.AdvanceIfCurrentIsOperator(out OperatorToken op, Operator.Minus))
            {
                e = null;
                return restoreCursor(oldCursor);
            }

            // expr
            if (!TryExpression(out ExpressionAst expr))
            {
                e = null;
                return restoreCursor(oldCursor);
            }

            e = new NegateExpressionAst(op, expr);
            return true;
        }

        internal bool TryGroupExpression(out GroupExpressionAst e)
        {
            var oldCursor = _reader._cursor;

            // '(' expr ')'

            if (!_reader.AdvanceIfCurrentIsOperator(out OperatorToken leftParen, Operator.LeftParen))
            {
                e = null; return restoreCursor(oldCursor);
            }

            if (!TryExpression(out ExpressionAst expression))
            {
                e = null; return restoreCursor(oldCursor);
            }

            if (!_reader.AdvanceIfCurrentIsOperator(out OperatorToken rightParen, Operator.RightParen))
            {
                e = null; return restoreCursor(oldCursor);
            }

            e = new GroupExpressionAst(leftParen, expression, rightParen);
            return true;
        }

        internal bool TryIdentExpression(out IdentExpressionAst e)
        {
            var oldCursor = _reader._cursor;

            // IDENT
            if (!_reader.AdvanceIfCurrentIsType(out IdentifierToken identifier))
            {
                e = null; return restoreCursor(oldCursor);
            }

            e = new IdentExpressionAst(identifier);
            return true;
        }

        internal bool TryLiteralExpression(out LiteralExpressionAst e)
        {
            var oldCursor = _reader._cursor;

            if (!_reader.AdvanceIfCurrentIsType(out LiteralToken literalToken))
            {
                e = null; return restoreCursor(oldCursor);
            }

            e = new LiteralExpressionAst(literalToken);
            return true;
        }

        internal bool TryCallExpression(out CallExpressionAst e)
        {
            var oldCursor = _reader._cursor;

            // IDENT '(' call_param_list? ')'

            if (!_reader.AdvanceIfCurrentIsType(out IdentifierToken identifier))
            {
                e = null; return restoreCursor(oldCursor);
            }

            if (!_reader.AdvanceIfCurrentIsOperator(out OperatorToken leftParen, Operator.LeftParen))
            {
                e = null; return restoreCursor(oldCursor);
            }

            // Optional
            TryParamList(out CallParamListAst paramList);

            if (!_reader.AdvanceIfCurrentIsOperator(out OperatorToken rightParen, Operator.RightParen))
            {
                e = null; return restoreCursor(oldCursor);
            }

            e = new CallExpressionAst(identifier, leftParen, paramList, rightParen);
            return true;
        }

        internal bool TryParamList(out CallParamListAst e)
        {
            var oldCursor = _reader._cursor;

            // expr (',' expr)*

            var argList = new List<ExpressionAst>();

            if (!TryExpression(out ExpressionAst arg0))
            {
                e = null; return restoreCursor(oldCursor);
            }

            argList.Add(arg0);

            while (true)
            {
                if (!_reader.AdvanceIfCurrentIsOperator(out OperatorToken _, Operator.Comma))
                    break;

                if (!TryExpression(out ExpressionAst arg))
                {
                    e = null; return restoreCursor(oldCursor);
                }

                argList.Add(arg);
            }

            e = new CallParamListAst(argList);
            return true;
        }

        internal bool TryAssignExpression(out AssignExpressionAst e)
        {
            var oldCursor = _reader._cursor;

            // IDENT '=' expr

            if (!_reader.AdvanceIfCurrentIsType(out IdentifierToken identifier))
            {
                e = null;
                return restoreCursor(oldCursor);
            }

            if (_reader.AdvanceIfCurrentIsOperator(out OperatorToken assign, Operator.Assign))
            {
                e = null;
                return restoreCursor(oldCursor);
            }

            if (!TryExpression(out ExpressionAst expression))
            {
                e = null;
                return restoreCursor(oldCursor);
            }

            e = new AssignExpressionAst(identifier, assign, expression);
            return true;
        }

        internal bool TryExpression(out ExpressionAst e)
        {
            var oldCursor = _reader._cursor;

            ExpressionAst expression = null;

            if (TryOperatorExpression(out OperatorExpressionAst operatorExpression))
            {
                expression = operatorExpression;
            }
            else if (TryNegateExpression(out NegateExpressionAst negateExpression))
            {
                expression = negateExpression;
            }
            else if (TryAssignExpression(out AssignExpressionAst assignExpression))
            {
                expression = assignExpression;
            }
            else if (TryCallExpression(out CallExpressionAst callExpression))
            {
                expression = callExpression;
            }
            else if (TryLiteralExpression(out LiteralExpressionAst literalExpression))
            {
                expression = literalExpression;
            }
            else if (TryIdentExpression(out IdentExpressionAst identExpression))
            {
                expression = identExpression;
            }
            else if (TryGroupExpression(out GroupExpressionAst groupExpression))
            {
                expression = groupExpression;
            }

            if (expression == null)
            {
                e = null;
                return restoreCursor(oldCursor);
            }

            e = expression;
            return true;
        }

    }
}
