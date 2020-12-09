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
        public IReadOnlyList<FunctionSymbol> FunctionsView => Functions.AsReadOnly();

        private List<VariableSymbol> GlobalVariables { get; } = new List<VariableSymbol>();
        public IReadOnlyList<VariableSymbol> GlobalVariablesView => GlobalVariables.AsReadOnly();

        public GlobalBuilder()
        {
            _registerStartFunction();
        }

        private void _registerStartFunction()
        {
            var function = new FunctionSymbol("_start", DataType.Void, new DataType[0]);
            RegisterFunction(function);
        }

        public void RegisterFunction(FunctionSymbol function)
        {
            if (function is null)
                throw new ArgumentNullException(nameof(function));

            var nameAt = RegisterStringConstant(function.Name);

            var builder = new FunctionBuilder(Functions.Count, nameAt);

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

        private int stringCounter { get; set; } = 0;

        public int RegisterStringConstant(string value)
        {
            var name = "__str_const_" + stringCounter;
            stringCounter++;

            var variable = new VariableSymbol(name, true, true, DataType.String);
            RegisterGlobalVariable(variable);
            variable.GlobalVariableBuilder.StringConstantValue = value;

            return variable.GlobalVariableBuilder.Id;
        }
    }
}
