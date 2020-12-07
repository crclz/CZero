using CZero.Intermediate.Symbols;
using System;
using System.Collections.Generic;
using System.Text;

namespace CZero.Intermediate.Test.SemanticCheck.Integration
{
    class HookableScope : SymbolScope
    {
        public Action<Symbol> HookFunction { get; set; }

        public HookableScope()
        {

        }

        public HookableScope(HookableScope parent) : base(parent)
        {

        }

        public override void AddSymbol(FunctionSymbol symbol)
        {
            HookFunction(symbol);
            base.AddSymbol(symbol);
        }

        public override void AddSymbol(VariableSymbol symbol)
        {
            HookFunction(symbol);
            base.AddSymbol(symbol);
        }

        public override SymbolScope CreateChildScope()
        {
            return new HookableScope(this)
            {
                HookFunction = HookFunction
            };
        }
    }
}
