using System;
using System.Collections.Generic;
using System.Text;

namespace CZero.Intermediate.Builders
{
    class WhileBuilder
    {
        public WhileBuilder ParentWhile { get; }

        public WhileBuilder(WhileBuilder parent)
        {
            ParentWhile = parent;
        }
    }
}
