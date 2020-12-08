using CZero.Intermediate.Symbols;
using System;
using System.Collections.Generic;
using System.Text;

namespace CZero.Intermediate.Builders
{
    class GlobalVariableBuilder
    {
        public int Id { get; }

        public List<object[]> LoadValueInstructions { get; set; }

        public GlobalVariableBuilder(int id)
        {
            Id = id;
        }
    }
}
