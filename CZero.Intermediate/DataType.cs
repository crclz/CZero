using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace CZero.Intermediate
{
    enum DataType
    {
        Void, Long, Double, Char, String, Bool
    }

    static class DataTypeHelper
    {
        public static bool IsLongOrDouble(DataType type) =>
            type == DataType.Double || type == DataType.Long;

        public static bool IsValidParamType(DataType type)
        {
            Guard.Against.OutOfRange(type, nameof(type));
            return type == DataType.Long || type == DataType.Double;
        }

        public static bool IsValidReturnType(DataType type)
        {
            Guard.Against.OutOfRange(type, nameof(type));
            return type == DataType.Long || type == DataType.Double || type == DataType.Void;
        }
    }
}
