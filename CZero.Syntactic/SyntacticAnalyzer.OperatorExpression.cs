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

            if (TryAssignExpression(out AssignExpressionAst assignExpression))
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

        internal bool TryGoodFactor(out GoodFactorAst goodFactor)
        {
            var oldCursor = _reader._cursor;

            // { - } strong_factor

            var negatives = new List<OperatorToken>();

            while (true)
            {
                if (!_reader.AdvanceIfCurrentIsOperator(out OperatorToken op, Operator.Minus))
                    break;
                negatives.Add(op);
            }

            if (!TryStrongFactor(out StrongFactorAst strongFactor))
            {
                goodFactor = null;
                return restoreCursor(oldCursor);
            }

            goodFactor = new GoodFactorAst(negatives, strongFactor);
            return true;
        }

        internal bool TryFactor(out FactorAst factorAst)
        {
            var oldCursor = _reader._cursor;

            // strong_factor { as ty} // ty -> IDENT

            if (!TryGoodFactor(out GoodFactorAst goodFactor))
            {
                factorAst = null;
                return restoreCursor(oldCursor);
            }

            var asTypeList = new List<(KeywordToken AsToken, IdentifierToken Type)>();

            while (true)
            {
                if (!_reader.CurrentIsType(out KeywordToken asToken))
                    break;
                if (asToken.Keyword != Keyword.As)
                    break;

                _reader.Advance();// do not forget

                if (!_reader.AdvanceIfCurrentIsType(out IdentifierToken identifier))
                {
                    // If as list is broken, just roll back a little.
                    // The function and previous list items are still valid
                    _reader.SetCursor(_reader._cursor - 1);
                    break;
                }

                asTypeList.Add((asToken, identifier));
            }

            factorAst = new FactorAst(goodFactor, asTypeList);
            return true;
        }

        internal bool TryTerm(out TermAst termAst)
        {
            var oldCursor = _reader._cursor;

            // -> factor { *|/ factor }

            if (!TryFactor(out FactorAst factor))
            {
                termAst = null;
                return restoreCursor(oldCursor);
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
                    _reader.SetCursor(_reader._cursor - 1);
                    break;
                }

                opFactorList.Add((op, oneFactor));
            }

            termAst = new TermAst(factor, opFactorList);
            return true;
        }

        internal bool TryWeakTerm(out WeakTermAst weakTerm)
        {
            var oldCursor = _reader._cursor;

            // term { +|- term }

            if (!TryTerm(out TermAst firstTerm))
            {
                weakTerm = null;
                return restoreCursor(oldCursor);
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
                    _reader.SetCursor(_reader._cursor - 1);
                    break;
                }

                opTermList.Add((op, term));
            }

            weakTerm = new WeakTermAst(firstTerm, opTermList);
            return true;
        }

        internal bool TryOperatorExpression(out OperatorExpressionAst e)
        {
            var oldCursor = _reader._cursor;

            // weak_term { 比较符 weak_term }

            if (!TryWeakTerm(out WeakTermAst firstWeakTerm))
            {
                e = null;
                return restoreCursor(oldCursor);
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
                    _reader.SetCursor(_reader._cursor - 1);
                    break;
                }

                opTerms.Add((op, term));
            }

            e = new OperatorExpressionAst(firstWeakTerm, opTerms);
            return true;
        }
    }
}
