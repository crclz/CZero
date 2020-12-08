using System;
using System.Collections.Generic;
using System.Text;

namespace CZero.Intermediate.Builders
{
    class LocalLocation
    {
        public bool IsArgument { get; }
        public int Id { get; }

        public LocalLocation(bool isArgument, int id)
        {
            IsArgument = isArgument;
            Id = id;
        }
    }
}
