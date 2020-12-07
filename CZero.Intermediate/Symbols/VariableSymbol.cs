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

        // 1. 初始值可能要经过计算，所以存不了
        // 2. 也不需要初始值有没有
        //public object InitialValue { get; }

        public VariableSymbol(
            string name, bool isGlobal, bool isConstant,
            DataType type) : base(name)
        {
            IsGlobal = isGlobal;
            IsConstant = isConstant;
            Type = type;
        }

    }
}
