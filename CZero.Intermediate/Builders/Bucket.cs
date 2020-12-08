using Ardalis.GuardClauses;
using CZero.Syntactic;
using System;
using System.Collections.Generic;
using System.Text;

namespace CZero.Intermediate.Builders
{
    class Bucket
    {
        public List<object[]> InstructionList { get; set; } = new List<object[]>();

        public void Add(string singleOpCode)
        {
            Guard.Against.Null(singleOpCode, nameof(singleOpCode));
            Add(new object[] { singleOpCode });
        }

        public void Add(params object[] instruction)
        {
            Guard.Against.Null(instruction, nameof(instruction));
            Guard.Against.NullElement(instruction, nameof(instruction));

            InstructionList.Add(instruction);
        }

        public void AddInstruction(object[] instruction)
        {
            Guard.Against.Null(instruction, nameof(instruction));
            Guard.Against.NullElement(instruction, nameof(instruction));

            InstructionList.Add(instruction);
        }

        public void AddRange(List<object[]> instructions)
        {
            InstructionList.AddRange(instructions);
        }

        public List<object[]> Pop()
        {
            var instructions = InstructionList;
            InstructionList = new List<object[]>();
            return instructions;
        }
    }
}
