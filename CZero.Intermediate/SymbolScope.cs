using Ardalis.GuardClauses;
using CZero.Intermediate.Symbols;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace CZero.Intermediate
{
    class SymbolScope
    {
        private SymbolScope ParentScope { get; }
        private Dictionary<string, Symbol> Symbols { get; } = new Dictionary<string, Symbol>();

        /// <summary>
        /// This should be used to initialize root scope
        /// </summary>
        public SymbolScope()
        {

        }

        public SymbolScope(SymbolScope parentScope)
        {
            ParentScope = parentScope ?? throw new ArgumentNullException(nameof(parentScope));
        }

        public SymbolScope CreateChildScope()
        {
            return new SymbolScope(this);
        }

        public void AddSymbol(VariableSymbol symbol)
        {
            // TODO: check if the symbol is keyword

            Guard.Against.Null(symbol, nameof(symbol));

            // 覆盖性：变量覆盖一切，函数不能覆盖任何
            if (FindSymbolShallow(symbol.Name, out Symbol _))
                throw new ArgumentException("Duplicated symbol name");

            var success = Symbols.TryAdd(symbol.Name, symbol);
            Debug.Assert(success);
        }

        public void AddSymbol(FunctionSymbol symbol)
        {
            // TODO: check if the symbol is keyword

            Guard.Against.Null(symbol, nameof(symbol));
            // 覆盖性：变量覆盖一切，函数不能覆盖任何

            if (FindSymbolDeep(symbol.Name, out Symbol symbol1))
                throw new ArgumentException("Duplicated symbol name");

            var success = Symbols.TryAdd(symbol.Name, symbol);
            Debug.Assert(success);
        }

        public bool FindSymbolShallow(string name, out Symbol symbol)
        {
            if (Symbols.ContainsKey(name))
            {
                symbol = Symbols[name];
                return true;
            }
            else
            {
                symbol = null;
                return false;
            }
        }

        public bool FindSymbolDeep(string name, out Symbol symbol)
        {
            if (FindSymbolShallow(name, out Symbol shallowSymbol))
            {
                Debug.Assert(shallowSymbol != null);
                symbol = shallowSymbol;
                return true;
            }

            Debug.Assert(shallowSymbol == null);

            if (ParentScope != null)
            {
                if (ParentScope.FindSymbolDeep(name, out Symbol deepSymbol))
                {
                    Debug.Assert(deepSymbol != null);
                    symbol = deepSymbol;
                    return true;
                }
            }

            symbol = null;
            return false;
        }
    }
}
