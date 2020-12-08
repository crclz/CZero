using Ardalis.GuardClauses;
using CZero.Intermediate.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CZero.Intermediate.Symbols
{
    class FunctionSymbol : Symbol
    {
        public DataType ReturnType { get; }
        public IReadOnlyList<DataType> ParamTypes { get; }

        public FunctionBuilder Builder { get; set; }

        public FunctionSymbol(string name, DataType returnType,
            IEnumerable<DataType> paramTypes) : base(name)
        {
            Guard.Against.OutOfRange(returnType, nameof(returnType));
            Guard.Against.Null(paramTypes, nameof(paramTypes));

            if (!DataTypeHelper.IsValidReturnType(returnType))
                throw new ArgumentException(nameof(returnType));

            foreach (var paramType in paramTypes)
            {
                if (!DataTypeHelper.IsValidParamType(paramType))
                    throw new ArgumentException(nameof(paramTypes));
            }

            ReturnType = returnType;
            ParamTypes = paramTypes.ToList();
        }

        // TODO: test this function
        public bool IsParamTypeMatch(IReadOnlyList<DataType> typeList)
        {
            if (typeList.Count != ParamTypes.Count)
                return false;

            for (int i = 0; i < ParamTypes.Count; i++)
            {
                if (ParamTypes[i] != typeList[i])
                    return false;
            }

            return true;
        }
    }
}
