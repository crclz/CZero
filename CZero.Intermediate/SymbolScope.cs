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

        public SymbolScope(SymbolScope parentScope)
        {
            ParentScope = parentScope ?? throw new ArgumentNullException(nameof(parentScope));
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

            if (FindSymbolDeep(name, out Symbol deepSymbol))
            {
                Debug.Assert(deepSymbol != null);
                symbol = deepSymbol;
                return true;
            }

            symbol = null;
            return false;
        }
    }
}
