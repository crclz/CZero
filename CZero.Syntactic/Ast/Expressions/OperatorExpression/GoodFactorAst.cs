using Ardalis.GuardClauses;
using CZero.Lexical.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace CZero.Syntactic.Ast.Expressions.OperatorExpression
{
    public class GoodFactorAst : Ast
    {
        public bool IsNegative { get; }
        public StrongFactorAst StrongFactor { get; }

        public GoodFactorAst(IEnumerable<OperatorToken> negatives, StrongFactorAst strongFactor)
        {
            Guard.Against.Null(negatives, nameof(negatives));
            Guard.Against.NullElement(negatives, nameof(negatives));

            StrongFactor = Guard.Against.Null(strongFactor, nameof(strongFactor));

            // Check all is Minus

            IsNegative = false;

            foreach (var op in negatives)
            {
                IsNegative = !IsNegative;

                if (op.Value != Operator.Minus)
                    throw new ArgumentException();
            }
        }
    }
}
