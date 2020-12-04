using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CZero.Syntactic.Ast.Expressions
{
    // expr (',' expr)* 爷懒得管逗号了
    public class CallParamListAst : Ast
    {
        public IReadOnlyList<ExpressionAst> Parameters { get; }

        public CallParamListAst(IEnumerable<ExpressionAst> parameters)
        {
            Parameters = parameters.ToList() ?? throw new ArgumentNullException(nameof(parameters));

            Guard.Against.NullElement(Parameters, nameof(parameters));
        }
    }
}
