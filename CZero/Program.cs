using CZero.Syntactic.Test.IntegrationTest;
using System;
using System.IO;

namespace CZero
{
    class Program
    {
        static void Main(string[] args)
        {
            var sourceFile = @"C:\Users\chr\Desktop\compiling\CZero\CZero.Syntactic.Test\IntegrationTest\Samples\sample1.c0";
            var sourceCode = File.ReadAllText(sourceFile);
            var ast = AstIntegrationTest.GetAst(sourceCode);

            AstTraverse.Traverse(ast);
        }
    }
}
