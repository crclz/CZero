using CZero.Intermediate;
using CZero.Intermediate.Symbols;
using CZero.Lexical;
using CZero.Syntactic;
using CZero.Syntactic.Ast;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CZero
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {

                // args: input output
                var inputPath = args[0];
                var outputPath = args[1];

                var sourceCode = File.ReadAllText(inputPath);

                var ast = GetAst(sourceCode);

                var rootScope = new SymbolScope();

                var generator = new IntermediateCodeGenerator(rootScope);
                generator.CodeGenerationEnabled = true;
                generator.ReturnCheckEnabled = true;

                generator.ProcessProgram(ast);

                // Generate assembly code
                var asm = new AssemblyCodeGenerator(generator.GlobalBuilder);

                var codeLines = asm.Generate();

                // Print asm code
                foreach (var line in codeLines)
                    Console.WriteLine(line);

                // Write machine code
                var exeData = asm.Executable.GetData();
                File.WriteAllBytes(outputPath, exeData);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                Environment.Exit(66666);
            }

        }

        public static ProgramAst GetAst(string sourceCode)
        {
            var lexer = new Lexer(sourceCode);
            var tokens = lexer.Parse().ToList();

            var tokenReader = new TokenReader(tokens);
            var syntacticAnalyzer = new SyntacticAnalyzer(tokenReader);

            var ast = syntacticAnalyzer.AnalyzeProgram();

            if (!tokenReader.ReachedEnd)
                throw new SyntacticException("Syntactic error detected. but cannot tell where.");

            return ast;
        }
    }
}
