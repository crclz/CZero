using Ardalis.GuardClauses;
using CZero.Lexical.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CZero.Syntactic.Ast.Expressions.OperatorExpression
{
    public class FactorAst : Ast
    {
        public GoodFactorAst GoodFactor { get; }
        public IReadOnlyList<(KeywordToken AsToken, IdentifierToken TypeToken)> AsTypeList { get; }

        public FactorAst(GoodFactorAst goodFactor,
            IEnumerable<(KeywordToken AsToken, IdentifierToken TypeToken)> asTypeList)
        {
            Guard.Against.Null(goodFactor, nameof(goodFactor));
            Guard.Against.Null(asTypeList, nameof(asTypeList));

            var asTypeListCopy = asTypeList.ToList();

            foreach (var asType in asTypeListCopy)
            {
                Guard.Against.Null(asType, nameof(asType));
                Guard.Against.Null(asType.AsToken, nameof(asType.AsToken));
                Guard.Against.Null(asType.TypeToken, nameof(asType.TypeToken));

                if (asType.AsToken.Keyword != Keyword.As)
                    throw new ArgumentException("One of the AsToken is not Keyword.As");
            }

            // Check is ok
            GoodFactor = goodFactor;
            AsTypeList = asTypeListCopy;
        }
    }
}
