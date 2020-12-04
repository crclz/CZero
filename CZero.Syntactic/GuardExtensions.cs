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
    }
}
