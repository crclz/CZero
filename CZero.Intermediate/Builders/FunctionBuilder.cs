using CZero.Intermediate.Symbols;
using System;
using System.Collections.Generic;
using System.Text;

namespace CZero.Intermediate.Builders
{
    class FunctionBuilder
    {
        public int Id { get; }
        public int NameAt { get; }
        public List<VariableSymbol> Arguments { get; } = new List<VariableSymbol>();
        public List<VariableSymbol> LocalVariables { get; } = new List<VariableSymbol>();

        public Bucket Bucket { get; } = new Bucket();

        public FunctionBuilder(int id, int nameAt)
        {
            Id = id;
            NameAt = nameAt;
        }

        public void RegisterArgument(VariableSymbol variable)
        {
            if (variable is null)
                throw new ArgumentNullException(nameof(variable));

            if (variable.IsGlobal)
                throw new ArgumentException(nameof(variable));

            variable.LocalLocation = new LocalLocation(isArgument: true, Arguments.Count);
            Arguments.Add(variable);
        }

        public void RegisterLocalVariable(VariableSymbol variable)
        {
            if (variable is null)
                throw new ArgumentNullException(nameof(variable));

            if (variable.IsGlobal)
                throw new ArgumentException(nameof(variable));

            variable.LocalLocation = new LocalLocation(isArgument: false, LocalVariables.Count);
            LocalVariables.Add(variable);
        }
    }
}
