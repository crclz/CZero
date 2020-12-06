using CZero.Lexical;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace CZero.Syntactic.Test.IntegrationTest
{
    public class AstIntegrationTest
    {
        public string SampleDirectory => new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory)
               .Parent.Parent.Parent.FullName + "/IntegrationTest/Samples/";

        public static string loadSample(string sampleName)
        {
            var testProjectRoot = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory)
                .Parent.Parent.Parent.FullName;

            var sampleFilename = testProjectRoot + "/IntegrationTest/Samples/" + sampleName;

            return File.ReadAllText(sampleFilename);
        }

        public static Ast.Ast GetAst(string sourceCode)
        {
            var lexer = new Lexer(sourceCode);
            var tokens = lexer.Parse().ToList();

            var tokenReader = new TokenReader(tokens);
            var syntacticAnalyzer = new SyntacticAnalyzer(tokenReader);

            var ast = syntacticAnalyzer.AnalyzeProgram();
            Assert.True(tokenReader.ReachedEnd);

            return ast;
        }

        /*
         * Known Bug (Won't Fix):
         * Traverse can only go into IEnumerable<Ast>,
         * it cannot go into something like OpFactors, which is IEnumerable<(xxx, FactorAst)>.
         * 
         * So, to avoid triggering this bug, do not detect CallExpression in OperationExpression,
         * like 2*pow(2,3)
         */

        [Fact]
        void TestSamples()
        {
            var sampleFilenameList = Directory.GetFiles(SampleDirectory);
            Assert.NotEmpty(sampleFilenameList);

            foreach (var samplePath in sampleFilenameList)
            {
                var sampleShortName = new FileInfo(samplePath).Name;

                var sourceCode = File.ReadAllText(samplePath);

                var ast = GetAst(sourceCode);

                var traverse = new AstTraverse();
                traverse.Traverse(ast);

                var astStatistics = traverse.GetStatistics().ToDictionary(p => p.Name, p => p.Count);

                var codeStatistics = SampleStatistics.GenerateStatistics(sourceCode);
                foreach (var (name, count) in codeStatistics)
                {
                    if (!astStatistics.ContainsKey(name))
                    {
                        throw new Exception($"[{sampleShortName}] <{name}> is not found");
                    }
                    else if (astStatistics[name] != count)
                    {
                        throw new Exception(
                            $"[{sampleShortName}] <{name}> Expect: {count}. Actual: {astStatistics[name]}");
                    }
                    else
                    {
                        // ok
                    }
                }

            }
        }
    }
}
