using System;
using System.Collections.Generic;
using System.Text;

namespace CZero.Intermediate.Symbols
{
    abstract class Symbol
    {
        public string Name { get; }

        protected Symbol(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }
}
