using System;
using System.Collections.Generic;
using System.Text;

namespace CZero.Intermediate.Symbols
{
    class FunctionSymbol : Symbol
    {
        FunctionBuilder Builder { get; }

        public FunctionSymbol(string name, IEnumerable<DataType> returnTypes) : base(name)
        {
        }
    }
}
