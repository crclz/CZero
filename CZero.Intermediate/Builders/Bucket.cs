using Ardalis.GuardClauses;
using CZero.Intermediate.Instructions;
using CZero.Syntactic;
using System;
using System.Collections.Generic;
using System.Text;

namespace CZero.Intermediate.Builders
{
    class Bucket
    {
        public List<Instruction> InstructionList { get; set; } = new List<Instruction>();

        public void AddSingle(string singleOpCode)
        {
            Guard.Against.Null(singleOpCode, nameof(singleOpCode));
            Add(new Instruction(new object[] { singleOpCode }));
        }

        public void Add(Instruction instruction)
        {
            Guard.Against.Null(instruction, nameof(instruction));

            InstructionList.Add(instruction);
        }

        public void AddRange(List<Instruction> instructions)
        {
            foreach (var instruction in instructions)
                Add(instruction);
        }

        public List<Instruction> Pop()
        {
            var instructions = InstructionList;
            InstructionList = new List<Instruction>();
            return instructions;
        }
    }
}
