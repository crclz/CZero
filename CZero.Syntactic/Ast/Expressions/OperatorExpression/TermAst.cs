using Ardalis.GuardClauses;
using CZero.Lexical.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CZero.Syntactic.Ast.Expressions.OperatorExpression
{
    public class TermAst : Ast
    {
        public FactorAst Factor { get; }
        public IReadOnlyList<(OperatorToken Op, FactorAst Factor)> OpFactors { get; }

        public TermAst(FactorAst factor, IEnumerable<(OperatorToken Op, FactorAst Factor)> opFactors)
        {
            Guard.Against.Null(factor, nameof(factor));
            Guard.Against.Null(opFactors, nameof(opFactors));

            var opFactorsCopy = opFactors.ToList();
            foreach (var (op, fac) in opFactorsCopy)
            {
                Guard.Against.Null(op, nameof(op));
                Guard.Against.Null(fac, nameof(fac));

                // op should be *|/
                if (op.Value != Operator.Mult && op.Value != Operator.Divide)
                    throw new ArgumentException();
            }


            Factor = factor;
            OpFactors = opFactorsCopy;
        }
    }
}
