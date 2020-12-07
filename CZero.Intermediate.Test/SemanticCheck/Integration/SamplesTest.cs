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
    public class SamplesTest
    {
        public ITestOutputHelper TestOutput { get; }

        public SamplesTest(ITestOutputHelper testOutput)
        {
            TestOutput = testOutput;
        }

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

        public static List<(string, string)> GetDeclarationStatisticsFromSource(string sourceCode)
        {
            var matches = Regex.Matches(sourceCode, "<(.+?):(.+?)>");
            return matches.Select(p => (p.Groups[1].Value, p.Groups[2].Value)).ToList();
        }

        [Fact]
        void CheckAllSamples()
        {
            var samples = Directory.GetFiles(SampleDirectory);

            Assert.NotEmpty(samples);

            foreach (var sampleFile in samples)
            {
                var sourceCode = File.ReadAllText(sampleFile);
                var sourceDeclStat = GetDeclarationStatisticsFromSource(sourceCode);

                var isBadSample = sourceCode.Contains("<bad>");

                var ast = GetAst(sourceCode);

                var rootScope = new HookableScope();
                var actualStat = new List<(string, string)>();
                rootScope.HookFunction = symbol =>
                {
                    if (symbol is FunctionSymbol function)
                        actualStat.Add((function.Name, "fn"));
                    else if (symbol is VariableSymbol variable)
                    {
                        var typeString = variable.Type switch
                        {
                            DataType.Long => "int",
                            DataType.Double => "double",
                            _ => throw new Exception()
                        };
                        if (!variable.IsConstant)
                            actualStat.Add((variable.Name, typeString));
                        else
                            actualStat.Add((variable.Name, "const " + typeString));
                    }
                };

                var generator = new IntermediateCodeGenerator(rootScope);

                if (isBadSample)
                {
                    Assert.Throws<SemanticException>(() => generator.ProcessProgram(ast));
                }
                else
                {
                    generator.ProcessProgram(ast);

                    // Scope are pushed and poped corrctly
                    Assert.Equal(rootScope, generator.SymbolScope);

                    // Check the declaration stat
                    Assert.Equal(sourceDeclStat.Count, actualStat.Count);

                    for (int i = 0; i < actualStat.Count; i++)
                    {
                        Assert.Equal(sourceDeclStat[i], actualStat[i]);
                    }
                }


            }

            TestOutput.WriteLine($"{samples.Length} samples tested");
        }
    }
}
