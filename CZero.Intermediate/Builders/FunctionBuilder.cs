using CZero.Intermediate.Symbols;
using System;
using System.Collections.Generic;
using System.Text;

namespace CZero.Intermediate.Builders
{
    class FunctionBuilder
    {
        public int Id { get; }

        private List<VariableSymbol> LocalVariables { get; } = new List<VariableSymbol>();
        public int LocalVariableCount => LocalVariables.Count;

    }
}
