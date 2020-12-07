using CZero.Intermediate.Builders;
using CZero.Intermediate.Symbols;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("CZero.Intermediate.Test")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace CZero.Intermediate
{
    partial class IntermediateCodeGenerator
    {
        internal SymbolScope SymbolScope { get; set; }

        public FunctionSymbol CurrentFunction { get; private set; }
        public bool IsInFunction => CurrentFunction != null;

        public WhileBuilder CurrentWhile { get; private set; }
        public bool IsInWhile => CurrentWhile != null;

        public IntermediateCodeGenerator()
        {

        }

        public IntermediateCodeGenerator(SymbolScope symbolScope)
        {
            SymbolScope = symbolScope ?? throw new ArgumentNullException(nameof(symbolScope));
        }

        public void LeaveFunctionDefination()
        {
            if (!IsInFunction)
                throw new InvalidOperationException("Not in function. Cannot leave function");

            CurrentFunction = null;
        }

        public void EnterFunctionDefination(FunctionSymbol function)
        {
            if (IsInFunction)
                throw new InvalidOperationException(
                    $"Cannot enter function when in function {CurrentFunction.Name}");

            CurrentFunction = function;
        }

        public void LeaveWhileDefination()
        {
            if (!IsInWhile)
                throw new InvalidOperationException("Not in while. Cannot leave while");

            CurrentWhile = CurrentWhile.ParentWhile;
        }

        public void EnterWhileDefination(WhileBuilder whileBuilder)
        {
            if (whileBuilder.ParentWhile != CurrentWhile)
                throw new ArgumentException("Wrong parent relationships");

            CurrentWhile = whileBuilder;
        }
    }
}
