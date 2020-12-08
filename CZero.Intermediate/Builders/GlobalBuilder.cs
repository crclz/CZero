using Ardalis.GuardClauses;
using CZero.Intermediate.Symbols;
using System;
using System.Collections.Generic;
using System.Text;

namespace CZero.Intermediate.Builders
{
    class GlobalBuilder
    {
        private List<FunctionSymbol> Functions { get; } = new List<FunctionSymbol>();
        private List<VariableSymbol> GlobalVariables { get; } = new List<VariableSymbol>();

        public void RegisterFunction(FunctionSymbol function)
        {
            if (function is null)
                throw new ArgumentNullException(nameof(function));

            var builder = new FunctionBuilder(Functions.Count);

            function.Builder = builder;

            Functions.Add(function);
        }

        public void RegisterGlobalVariable(VariableSymbol variable)
        {
            if (variable is null)
                throw new ArgumentNullException(nameof(variable));

            if (!variable.IsGlobal)
                throw new ArgumentException(nameof(variable));

            var builder = new GlobalVariableBuilder(GlobalVariables.Count);

            variable.GlobalVariableBuilder = builder;

            GlobalVariables.Add(variable);
        }
    }
}
