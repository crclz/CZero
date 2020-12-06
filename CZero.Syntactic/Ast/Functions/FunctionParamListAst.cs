using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CZero.Syntactic.Ast.Functions
{
    public class FunctionParamListAst : Ast
    {
        // function_param (',' function_param)*

        public IReadOnlyList<FunctionParamAst> FunctionParams { get; }

        public FunctionParamListAst(IEnumerable<FunctionParamAst> functionParams)
        {
            Guard.Against.Null(functionParams, nameof(functionParams));
            Guard.Against.NullElement(functionParams, nameof(functionParams));

            FunctionParams = functionParams.ToList();
        }
    }
}
