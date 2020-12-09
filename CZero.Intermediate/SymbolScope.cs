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
        public SymbolScope ParentScope { get; }

        //public bool IsRoot => ParentScope == null;

        private Dictionary<string, Symbol> Symbols { get; } = new Dictionary<string, Symbol>();

        /// <summary>
        /// This should be used to initialize root scope
        /// </summary>
        public SymbolScope()
        {
            ConfigureSystemCalls();
        }

        public SymbolScope(SymbolScope parentScope)
        {
            ParentScope = parentScope ?? throw new ArgumentNullException(nameof(parentScope));
        }

        public virtual SymbolScope CreateChildScope()
        {
            return new SymbolScope(this);
        }

        public virtual void AddSymbol(VariableSymbol symbol)
        {
            // TODO: check if the symbol is keyword

            Guard.Against.Null(symbol, nameof(symbol));

            // 覆盖性：变量覆盖一切，函数不能覆盖任何
            if (FindSymbolShallow(symbol.Name, out Symbol _))
                throw new ArgumentException("Duplicated symbol name");

            var success = Symbols.TryAdd(symbol.Name, symbol);
            Debug.Assert(success);
        }

        public virtual void AddSymbol(FunctionSymbol symbol)
        {
            // TODO: check if the symbol is keyword

            Guard.Against.Null(symbol, nameof(symbol));
            // 覆盖性：变量覆盖一切，函数不能覆盖任何

            if (FindSymbolDeep(symbol.Name, out Symbol symbol1))
                throw new ArgumentException("Duplicated symbol name");

            var success = Symbols.TryAdd(symbol.Name, symbol);
            Debug.Assert(success);
        }

        // Do not want hook to detect system call configuration
        private void _addFuncSym(FunctionSymbol symbol)
        {
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

        private void ConfigureSystemCalls()
        {
            // 读入一个有符号整数
            //fn getint() -> int;
            _addFuncSym(new FunctionSymbol("getint", DataType.Long, new DataType[0]));

            // 读入一个浮点数
            //fn getdouble() -> double;
            _addFuncSym(new FunctionSymbol("getdouble", DataType.Double, new DataType[0]));

            // 读入一个字符
            //fn getchar() -> int;
            _addFuncSym(new FunctionSymbol("getchar", DataType.Long, new DataType[0]));

            // 输出一个整数
            //fn putint(int) -> void;
            _addFuncSym(new FunctionSymbol("putint", DataType.Void, new[] { DataType.Long }));

            // 输出一个浮点数
            //fn putdouble(double) -> void;
            _addFuncSym(new FunctionSymbol("putdouble", DataType.Void, new[] { DataType.Double }));

            // 输出一个字符
            //fn putchar(int) -> void;
            _addFuncSym(new FunctionSymbol("putchar", DataType.Void, new[] { DataType.Long }));

            // 将编号为这个整数的全局常量看作字符串输出
            //fn putstr(int) -> void;
            _addFuncSym(new FunctionSymbol("putstr", DataType.Void, new[] { DataType.String }));

            // 输出一个换行
            //fn putln() -> void;
            _addFuncSym(new FunctionSymbol("putln", DataType.Void, new DataType[0]));

        }
    }
}
