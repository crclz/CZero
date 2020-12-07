using System;
using System.Collections.Generic;
using System.Text;

namespace CZero.Intermediate.Symbols
{
    class VariableSymbol : Symbol
    {
        public bool IsGlobal { get; }
        public bool IsConstant { get; }
        public DataType Type { get; }
        public object InitialValue { get; }

        public bool HasInitialValue => InitialValue != null;

        public VariableSymbol(
            string name, bool isGlobal, bool isConstant,
            DataType type, object initialValue) : base(name)
        {
            if (isConstant && initialValue == null)
                throw new ArgumentException("Const should have initial value");

            if (initialValue != null && !DataTypeHelper.IsType(initialValue, type))
            {
                throw new ArgumentException(
                    $"Initial value ({initialValue.GetType()}) not match data type {type}");
            }

            IsGlobal = isGlobal;
            IsConstant = isConstant;
            Type = type;
            InitialValue = initialValue;
        }

    }
}
