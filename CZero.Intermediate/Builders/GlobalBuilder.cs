using CZero.Intermediate.Symbols;
using System;
using System.Collections.Generic;
using System.Text;

namespace CZero.Intermediate.Builders
{
    class GlobalBuilder
    {
        private List<FunctionSymbol> Functions { get; } = new List<FunctionSymbol>();
        private List<GlobalVariableBuilder> GlobalVariables { get; } = new List<GlobalVariableBuilder>();

    }
}
