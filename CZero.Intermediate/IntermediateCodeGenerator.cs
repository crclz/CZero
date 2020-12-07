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
        private SymbolScope SymbolScope { get; }

        public IntermediateCodeGenerator()
        {

        }

        public IntermediateCodeGenerator(SymbolScope symbolScope)
        {
            SymbolScope = symbolScope ?? throw new ArgumentNullException(nameof(symbolScope));
        }
    }
}
