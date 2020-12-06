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
            var sourceFile = @"C:\Users\chr\Desktop\compiling\CZero\CZero.Syntactic.Test\IntegrationTest\Samples\sample1.c0";
            var sourceCode = File.ReadAllText(sourceFile);
            var ast = AstIntegrationTest.GetAst(sourceCode);

            var traverse = new AstTraverse();
            traverse.Traverse(ast);

            //foreach (var (name, count) in traverse.GetStatistics())
            //{
            //    Console.WriteLine($"{name}: {count}");
            //}

            var astStatistics = traverse.GetStatistics().ToDictionary(p => p.Name, p => p.Count);

            var s2 = SampleStatistics.GenerateStatistics(sourceCode);
            foreach (var (name, count) in s2)
            {
                if (!astStatistics.ContainsKey(name))
                    Console.WriteLine($"not found: {name}");
                else if (astStatistics[name] != count)
                    Console.WriteLine($"[{name}] Expect: {count}. Actual: {astStatistics[name]}");
                else
                    Console.WriteLine($"[{name}] Expect: {count}. ok");

            }


        }
    }
}
