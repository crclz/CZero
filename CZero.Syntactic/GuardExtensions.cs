using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using System.Text;

namespace CZero.Syntactic
{
    public static class GuardExtensions
    {
        public static void NotType<T>(this IGuardClause guardClause, object input, string parameterName)
        {
            if (input.GetType() == typeof(T))
                throw new ArgumentException($"Argument {parameterName} should not be {typeof(T)}");
        }

        public static void NullElement<T>(
            this IGuardClause guardClause, IEnumerable<T> input, string parameterName)
        {
            foreach (var x in input)
            {
                if (x == null)
                    throw new ArgumentException($"Some element in {parameterName} is null");
            }
        }
    }
}
