using CZero.Intermediate.Symbols;
using CZero.Lexical;
using CZero.Syntactic;
using CZero.Syntactic.Ast;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Xunit;
using Xunit.Abstractions;

namespace CZero.Intermediate.Test.CodeGen
{
    public class SamplesTest
    {
        public ITestOutputHelper TestOutput { get; }

        public SamplesTest(ITestOutputHelper testOutput)
        {
            TestOutput = testOutput;
        }

        public string SampleDirectory => new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory)
            .Parent.Parent.Parent.FullName + "/CodeGen/Samples/";


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
        void JustNoException()
        {
            var samples = Directory.GetFiles(SampleDirectory);

            Assert.NotEmpty(samples);

            foreach (var sampleFile in samples)
            {
                var sourceCode = File.ReadAllText(sampleFile);

                var ast = GetAst(sourceCode);

                var rootScope = new SymbolScope();

                var generator = new IntermediateCodeGenerator(rootScope);
                generator.CodeGenerationEnabled = true;
                generator.ReturnCheckEnabled = true;

                generator.ProcessProgram(ast);

                // Scope are pushed and poped corrctly
                Assert.Equal(rootScope, generator.SymbolScope);

            }

            TestOutput.WriteLine($"{samples.Length} samples tested");
        }
    }
}
