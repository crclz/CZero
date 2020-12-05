using Ardalis.GuardClauses;
using CZero.Lexical.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CZero.Syntactic.Ast.Expressions.OperatorExpression
{
    public class OperatorExpressionAst : ExpressionAst
    {
        public WeakTermAst WeakTerm { get; }
        public IReadOnlyList<(OperatorToken Op, WeakTermAst WeakTerm)> OpTerms { get; }

        public OperatorExpressionAst(WeakTermAst weakTerm,
            IEnumerable<(OperatorToken Op, WeakTermAst Term)> opTerms)
        {
            Guard.Against.Null(weakTerm, nameof(WeakTerm));
            Guard.Against.Null(opTerms, nameof(opTerms));

            var opTermsCopy = opTerms.ToList();

            foreach (var (op, term) in opTerms)
            {
                Guard.Against.Null(op, nameof(op));
                Guard.Against.Null(term, nameof(term));

                if (!op.IsCompare)
                    throw new ArgumentException();
            }


            // Check passing
            WeakTerm = weakTerm;
            OpTerms = opTermsCopy;
        }
    }
}
