using Ardalis.GuardClauses;
using CZero.Intermediate.Instructions;
using CZero.Intermediate.Symbols;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

        /// <summary>
        /// Should be called at the end
        /// </summary>
        public void BuildStartFunction()
        {
            var start = Functions.Single(p => p.Name == "_start");

            foreach (var v in GlobalVariablesView)
            {
                var vb = v.GlobalVariableBuilder;

                if (vb.StringConstantValue != null)
                {
                    // string constant is already in the data after compiling
                }
                else
                {
                    if (!vb.HasInitialValue)
                    {
                        //code.Add($"# {v.Name}: no initial value");
                    }
                    else
                    {
                        //code.Add($"# {v.Name}: initial value setter code:");

                        // load-addr
                        start.Builder.Bucket.Add(Instruction.Pack("globa", vb.Id));

                        // init-expr
                        start.Builder.Bucket.AddRange(vb.LoadValueInstructions);

                        // store.64
                        start.Builder.Bucket.Add(new Instruction("store.64"));
                    }
                }
            }

            // call main
            var mainFunction = Functions.SingleOrDefault(p => p.Name == "main");
            if (mainFunction == null)
                throw new SemanticException("No main function");

            // Check main should have 0 arg and return void
            if (mainFunction.ParamTypes.Count > 0)
                throw new SemanticException("Function main should not have params");
            if (mainFunction.ReturnType != DataType.Void)
                throw new SemanticException("Function main's return type should be void");

            // args space and ret space
            var space = mainFunction.ParamTypes.Count;
            if (mainFunction.ReturnType != DataType.Void)
                space++;

            for (int i = 0; i < space; i++)
                start.Builder.Bucket.Add(Instruction.Pack("push", (long)0xDEADBEEF));

            start.Builder.Bucket.Add(Instruction.Pack("call", mainFunction.Builder.Id));
        }
    }
}
