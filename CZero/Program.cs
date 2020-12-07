using CZero.Intermediate.Test.SemanticCheck.Integration;
using CZero.Syntactic.Test.IntegrationTest;
using System;
using System.IO;
using System.Linq;

namespace CZero
{
    class Program
    {
        static void Main(string[] args)
        {
            var sourceFile = @"C:\Users\chr\Desktop\compiling\CZero\CZero.Intermediate.Test\SemanticCheck\Integration\Samples\1.c0";
            var sourceCode = File.ReadAllText(sourceFile);

            var ds = SamplesTest.GetDeclarationStatisticsFromSource(sourceCode);

        }
    }
}
