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

namespace CZero.Intermediate.Test.SemanticCheck.Integration
{
    public class LeaveTest
    {
        public ITestOutputHelper TestOutput { get; }

        public LeaveTest(ITestOutputHelper testOutput)
        {
            TestOutput = testOutput;
        }

        public string SampleDirectory => new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory)
            .Parent.Parent.Parent.FullName + "/SemanticCheck/Integration/LeaveSamples/";


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
        void CheckLeaveSamples()
        {
            var samples = Directory.GetFiles(SampleDirectory);

            Assert.NotEmpty(samples);

            foreach (var sampleFile in samples)
            {
                var sourceCode = File.ReadAllText(sampleFile);

                var isBadSample = sourceCode.Contains("<bad>");

                var ast = GetAst(sourceCode);

                var rootScope = new SymbolScope();

                var generator = new IntermediateCodeGenerator(rootScope);
                generator.ReturnCheckEnabled = true;// enable leave check

                if (isBadSample)
                {
                    Assert.Throws<SemanticException>(() => generator.ProcessProgram(ast));
                }
                else
                {
                    generator.ProcessProgram(ast);
                }
            }

            TestOutput.WriteLine($"{samples.Length} samples tested");
        }
    }
}
