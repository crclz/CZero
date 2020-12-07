using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace CZero.Intermediate
{
    enum DataType
    {
        Void, Long, Double, Char, String
    }

    static class DataTypeHelper
    {
        public static IReadOnlyDictionary<Type, DataType> DataTypeReference =
            new Dictionary<Type, DataType>
            {
                {typeof(long),DataType.Long},
                {typeof(double),DataType.Double},
                {typeof(char),DataType.Char},
                {typeof(string),DataType.String }
            };

        public static bool IsType(object value, DataType type)
        {
            if (!DataTypeReference.ContainsKey(value.GetType()))
                throw new ArgumentException();

            var actualType = DataTypeReference[value.GetType()];
            return type == actualType;
        }
    }
}
