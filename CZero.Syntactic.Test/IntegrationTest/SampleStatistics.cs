using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CZero.Syntactic.Test.IntegrationTest
{
    public static class SampleStatistics
    {
        public static List<(string Name, int Count)> GenerateStatistics(string sourceCode)
        {
            // e.g. <Function>
            var pattern = "<([a-zA-Z]+)>";
            var matches = Regex.Matches(sourceCode, pattern);
            var statistics = matches.Select(p => p.Groups[1].Value)
                .GroupBy(p => p).Select(g => (g.Key, g.Count())).ToList();
            return statistics;
        }
    }
}
