using CZero.Lexical.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace CZero.Syntactic.Ast.Functions
{
    public class FunctionParamAst : Ast
    {
        // 'const'? IDENT ':' ty

        ///<summary>Optional</summary>
        public KeywordToken Const { get; }

        public IdentifierToken Name { get; }

        public OperatorToken Colon { get; }

        public IdentifierToken Type { get; }

        public FunctionParamAst(KeywordToken @const, IdentifierToken name, OperatorToken colon, IdentifierToken type)
        {
            Const = @const;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Colon = colon ?? throw new ArgumentNullException(nameof(colon));
            Type = type ?? throw new ArgumentNullException(nameof(type));

            if (@const != null && @const.Keyword != Keyword.Const)
                throw new ArgumentNullException(nameof(@const));

            if (colon.Value != Operator.Colon)
                throw new ArgumentNullException(nameof(colon));
        }
    }
}
