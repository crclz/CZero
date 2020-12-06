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
        public static string loadSample(string sampleName)
        {
            var testProjectRoot = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory)
                .Parent.Parent.Parent.FullName;

            var sampleFilename = testProjectRoot + "/IntegrationTest/Samples/" + sampleName;

            return File.ReadAllText(sampleFilename);
        }

        public static Ast.Ast GetAst(string sourceCode)
        {
            var tokens = new Lexer(sourceCode).Parse().ToList();

            var tokenReader = new TokenReader(tokens);
            var syntacticAnalyzer = new SyntacticAnalyzer(tokenReader);

            var ast = syntacticAnalyzer.AnalyzeProgram();
            Assert.True(tokenReader.ReachedEnd);

            return ast;
        }
    }
}
