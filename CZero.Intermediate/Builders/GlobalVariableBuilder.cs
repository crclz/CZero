using CZero.Intermediate.Instructions;
using CZero.Intermediate.Symbols;
using System;
using System.Collections.Generic;
using System.Text;

namespace CZero.Intermediate.Builders
{
    class GlobalVariableBuilder
    {
        public int Id { get; }

        public List<Instruction> LoadValueInstructions { get; set; }

        public GlobalVariableBuilder(int id)
        {
            Id = id;
        }
    }
}
