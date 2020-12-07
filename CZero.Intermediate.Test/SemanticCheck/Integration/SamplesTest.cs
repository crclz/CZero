using CZero.Lexical;
using CZero.Syntactic;
using CZero.Syntactic.Ast;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace CZero.Intermediate.Test.SemanticCheck.Integration
{
    public class SamplesTest
    {
        public string SampleDirectory => new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory)
            .Parent.Parent.Parent.FullName + "/SemanticCheck/Integration/Samples/";

        public static ProgramAst GetAst(string sourceCode)
        {
            var lexer = new Lexer(sourceCode);
            var tokens = lexer.Parse().ToList();

            var tokenReader = new TokenReader(tokens);
            var syntacticAnalyzer = new SyntacticAnalyzer(tokenReader);

            var ast = syntacticAnalyzer.AnalyzeProgram();
            Assert.True(tokenReader.ReachedEnd);

            return ast;
        }

        [Fact]
        void CheckAllSamples()
        {
            var samples = Directory.GetFiles(SampleDirectory);

            Assert.NotEmpty(samples);

            foreach (var sampleFile in samples)
            {
                var sourceCode = File.ReadAllText(sampleFile);

                var isBadSample = sourceCode.Contains("<bad>");

                var ast = GetAst(sourceCode);

                var scope = new SymbolScope();

                var generator = new IntermediateCodeGenerator(scope);

                if (isBadSample)
                {
                    Assert.Throws<SemanticException>(() => generator.ProcessProgram(ast));
                }
                else
                {
                    generator.ProcessProgram(ast);
                }


            }
        }
    }
}
