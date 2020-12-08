using CZero.Intermediate.Builders;
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

        public GlobalVariableBuilder GlobalVariableBuilder { get; set; }
        public LocalLocation LocalLocation { get; set; }

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
