using Ardalis.GuardClauses;
using CZero.Syntactic.Ast.Functions;
using CZero.Syntactic.Ast.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CZero.Syntactic.Ast
{
    public class ProgramAst : Ast
    {
        // Not using: program -> decl_stmt* function*
        // using: program -> (decl_stmt | function)

        public IReadOnlyList<Ast> Elements { get; }

        public ProgramAst(IEnumerable<Ast> elements)
        {
            Guard.Against.Null(elements, nameof(elements));
            Guard.Against.NullElement(elements, nameof(elements));

            // (decl_stmt | function)
            foreach (var e in elements)
            {
                if (!(e is DeclarationStatementAst || e is FunctionAst))
                {
                    throw new ArgumentException();
                }
            }

            Elements = elements.ToList();
        }
    }
}
