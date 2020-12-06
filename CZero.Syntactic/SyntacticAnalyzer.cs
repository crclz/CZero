using CZero.Lexical.Tokens;
using CZero.Syntactic.Ast;
using CZero.Syntactic.Ast.Functions;
using CZero.Syntactic.Ast.Statements;
using System;
using System.Collections.Generic;
using System.Text;

namespace CZero.Syntactic
{
    public partial class SyntacticAnalyzer
    {
        private readonly TokenReader _reader;

        public SyntacticAnalyzer(TokenReader reader)
        {
            _reader = reader;
        }

        /// <summary>
        /// Remember to check reader.ReachedEnd.
        /// If not reach end, input is bad.
        /// </summary>
        public ProgramAst AnalyzeProgram()
        {
            // using: program -> (decl_stmt | function)

            var oldCursor = _reader._cursor;

            var elements = new List<Ast.Ast>();

            while (true)
            {
                var success = TryDeclarationStatement(out DeclarationStatementAst declarationStatement);
                if (success)
                {
                    elements.Add(declarationStatement);
                    continue;
                }

                success = TryFunction(out FunctionAst functionAst);
                if (success)
                {
                    elements.Add(functionAst);
                    continue;
                }

                break;
            }

            var programAst = new ProgramAst(elements);
            return programAst;
        }
    }
}
