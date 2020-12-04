using CZero.Lexical.Tokens;
using CZero.Syntactic.Ast.Expressions;
using CZero.Syntactic.Ast.Expressions.OperatorExpression;
using System;
using System.Collections.Generic;
using System.Text;

namespace CZero.Syntactic
{
    partial class SyntacticAnalyzer
    {
        internal bool TryStrongFactor(out StrongFactorAst strongFactorAst)
        {
            var oldCursor = _reader._cursor;

            ExpressionAst expression = null;

            if (TryNegateExpression(out NegateExpressionAst e))
            {
                expression = e;
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

            if (expression != null)
            {
                strongFactorAst = new StrongFactorAst(expression);
                return true;
            }
            else
            {
                strongFactorAst = null;
                return restoreCursor(oldCursor);
            }
        }

        internal bool TryFactor(out FactorAst factorAst)
        {
            // strong_factor { as ty} // ty -> IDENT

            if (!TryStrongFactor(out StrongFactorAst strongFactor))
            {
                factorAst = null;
                return false;
            }

            var asTypeList = new List<(KeywordToken AsToken, IdentifierToken Type)>();

            while (true)
            {
                if (!_reader.CurrentIsType(out KeywordToken asToken))
                    if (asToken.Keyword != Keyword.As)
                        break;

                _reader.Advance();// do not forget

                if (!_reader.AdvanceIfCurrentIsType(out IdentifierToken identifier))
                {
                    factorAst = null;
                    return false;
                }

                asTypeList.Add((asToken, identifier));
            }

            factorAst = new FactorAst(strongFactor, asTypeList);
            return false;
        }

        internal bool TryTerm(out TermAst termAst)
        {
            // -> factor { *|/ factor }

            if (!TryFactor(out FactorAst factor))
            {
                termAst = null;
                return false;
            }

            var opFactorList = new List<(OperatorToken Op, FactorAst Factor)>();

            while (true)
            {
                OperatorToken op;

                var success = _reader.AdvanceIfCurrentIsOperator(out op, Operator.Mult);
                if (!success)
                    success = _reader.AdvanceIfCurrentIsOperator(out op, Operator.Divide);

                if (!success)
                    break;

                if (!TryFactor(out FactorAst oneFactor))
                {
                    termAst = null;
                    return false;
                }

                opFactorList.Add((op, oneFactor));
            }

            termAst = new TermAst(factor, opFactorList);
            return true;
        }

        internal bool TryWeakTerm(out WeakTermAst weakTerm)
        {
            // term { +|- term }

            if (!TryTerm(out TermAst firstTerm))
            {
                weakTerm = null;
                return false;
            }

            var opTermList = new List<(OperatorToken Op, TermAst Term)>();

            while (true)
            {
                OperatorToken op;

                var success = _reader.AdvanceIfCurrentIsOperator(out op, Operator.Plus);
                if (!success)
                    success = _reader.AdvanceIfCurrentIsOperator(out op, Operator.Minus);

                if (!success)
                    break;

                if (!TryTerm(out TermAst term))
                {
                    weakTerm = null;
                    return false;
                }

                opTermList.Add((op, term));
            }

            weakTerm = new WeakTermAst(firstTerm, opTermList);
            return true;
        }

        internal bool TryOperatorExpression(out OperatorExpressionAst e)
        {
            // weak_term { 比较符 weak_term }

            if (!TryWeakTerm(out WeakTermAst firstWeakTerm))
            {
                e = null;
                return false;
            }

            var opTerms = new List<(OperatorToken Op, WeakTermAst Term)>();

            while (true)
            {
                if (!_reader.CurrentIsType(out OperatorToken op))
                    break;

                if (!op.IsCompare)
                    break;

                _reader.Advance();

                if (!TryWeakTerm(out WeakTermAst term))
                {
                    e = null;
                    return false;
                }

                opTerms.Add((op, term));
            }

            e = new OperatorExpressionAst(firstWeakTerm, opTerms);
            return true;
        }
    }
}
