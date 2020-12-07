using Ardalis.GuardClauses;
using CZero.Lexical.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CZero.Syntactic.Ast.Expressions.OperatorExpression
{
    public class WeakTermAst : Ast
    {
        public TermAst Term { get; }
        public IReadOnlyList<(OperatorToken Op, TermAst Term)> OpTerms { get; }

        protected WeakTermAst()
        {

        }

        public WeakTermAst(TermAst term, IEnumerable<(OperatorToken Op, TermAst Term)> opTerms)
        {
            Guard.Against.Null(term, nameof(term));
            Guard.Against.Null(opTerms, nameof(opTerms));

            var opTermsCopy = opTerms.ToList();
            foreach (var (op, t) in opTerms)
            {
                Guard.Against.Null(op, nameof(op));
                Guard.Against.Null(t, nameof(t));

                // +|-
                if (op.Value != Operator.Plus && op.Value != Operator.Minus)
                    throw new ArgumentException();
            }

            // Check pass

            Term = term;
            OpTerms = opTermsCopy;
        }
    }
}
