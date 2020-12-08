using Ardalis.GuardClauses;
using CZero.Syntactic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CZero.Intermediate.Instructions
{
    class Instruction
    {
        private static int SomeCounter { get; set; } = 1;

        public int Offset { get; set; } = -1;

        public IReadOnlyList<object> Parts { get; }

        public string Alias { get; }

        public Instruction(string singleOpCode) : this(new object[] { singleOpCode })
        {

        }

        public Instruction(IEnumerable<object> parts)
        {
            Guard.Against.Null(parts, nameof(parts));
            Guard.Against.NullElement(parts, nameof(parts));

            Parts = parts.ToArray();

            Alias = $".{SomeCounter}";
            SomeCounter++;
        }

        public static implicit operator Instruction(object[] parts)
        {
            return new Instruction(parts);
        }

        public static Instruction Pack(params object[] parts)
        {
            return new Instruction(parts);
        }

    }
}
